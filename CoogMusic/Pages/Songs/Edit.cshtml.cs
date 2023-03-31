using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.Songs
{
    public class EditModel : PageModel
    {
        public SongInfo songInfo = new SongInfo();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
            if (Request.Query.ContainsKey("id"))
            {
                if (int.TryParse(Request.Query["id"], out int songId))
                {
                    try
                    {
                        String connectionStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
                        using (MySqlConnection connection = new MySqlConnection(connectionStr))
                        {
                            connection.Open();
                            MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                            String sql = "SELECT * FROM song AS S WHERE S.id=@id";
                            using (MySqlCommand command = new MySqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@id", songId);
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        songInfo.songId = reader.GetInt32(0);
                                        songInfo.artistId = reader.GetInt32(1);
                                        songInfo.title = reader.GetString(2);
                                        songInfo.genre = reader.GetString(3);
                                        songInfo.CreateDate = reader.GetDateTime(6).ToString();
                                        //Add how to get the BLOB to mp3 file here
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error connecting to the database: " + ex.Message);
                    }
                }
                else
                {
                    errorMessage = "Invalid song ID: " + Request.Query["id"];
                }
            }
            else
            {
                errorMessage = "Song ID not specified";
            }
        }

        public async Task OnPostAsync()
        {
            songInfo.title = Request.Form["Title"];
            //songInfo.artistId = int.Parse(Request.Form["Artist"]);
            songInfo.genre = Request.Form["Genre"];
            //songInfo.songFile = Request.Form.Files["songfile"];
            byte[] songData;
            using (var memoryStream = new MemoryStream())
            {
                await songInfo.songFile.CopyToAsync(memoryStream);
                songData = memoryStream.ToArray();
            }
            // Input data into database
            try
            {
                String connectionStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    await connection.OpenAsync();
                    MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                    String sql = "UPDATE song SET  title= @Title, genre=@Genre WHERE id=@songId;";
                    // Add a way to know if the user owns the song (artistId=@ArtistId)
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Transaction = mySqlTransaction;

                        command.Parameters.AddWithValue("@songId", songInfo.songId);
                        //command.Parameters.AddWithValue("@ArtistId", songInfo.artistId);
                        //command.Parameters.AddWithValue("@Title", songInfo.title);
                        command.Parameters.AddWithValue("@Genre", songInfo.genre);
                        //command.Parameters.AddWithValue("@UploadDate", DateTime.UtcNow);

                        int affectedRows = await command.ExecuteNonQueryAsync();
                        if (affectedRows > 0)
                        {
                            // Display a success message or redirect to another page
                            successMessage = "New Song Added Correctly";
                            Response.Redirect("/Songs/");
                        }
                        else
                        {
                            errorMessage = "We experienced an error while adding to the database";
                        }
                        mySqlTransaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting MP3 file into database: " + ex.Message);
            }
        }
    }
}
