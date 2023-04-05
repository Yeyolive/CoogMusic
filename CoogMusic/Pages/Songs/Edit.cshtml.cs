using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.Songs
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SongInfo editSong = new SongInfo();
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
                        String connectionStr = _configuration.GetConnectionString("DefaultConnection");
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
                                        editSong.songId = reader.GetInt32("id"); 
                                        editSong.artistId = reader.GetInt32("artist_id");
                                        editSong.title = reader.GetString("title");
                                        editSong.genre = reader.GetString("genre");
                                        editSong.Explicit = reader.GetBoolean("explicit");
                                        editSong.CreateDate = reader.GetDateTime("upload_date").ToString();
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
            //songInfo.artistId = int.Parse(Request.Form["Artist"]);
            //songInfo.songFile = Request.Form.Files["songfile"];
            //byte[] songData;
            //using (var memoryStream = new MemoryStream())
            //{
            //    await songInfo.songFile.CopyToAsync(memoryStream);
            //    songData = memoryStream.ToArray();
            //}
            editSong.artistId = int.Parse(Request.Form["ArtistId"]);
            editSong.songId = int.Parse(Request.Form["SongId"]);
            editSong.Explicit = Request.Form["Explicit"] == "true";
            editSong.title = Request.Form["Title"];
            editSong.genre = Request.Form["Genre"];
            try
            {
                String connectionStr = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    await connection.OpenAsync();
                    MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                    String sql = "UPDATE song SET title=@Title, genre=@Genre, explicit=@Explicit WHERE id=@songId AND artist_id=@ArtistId;";
                    // Add a way to know if the user owns the song (artistId=@ArtistId)
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Transaction = mySqlTransaction;

                        command.Parameters.AddWithValue("@songId", editSong.songId);
                        command.Parameters.AddWithValue("@ArtistId", editSong.artistId);
                        command.Parameters.AddWithValue("@Title", editSong.title);
                        command.Parameters.AddWithValue("@Genre", editSong.genre);
                        command.Parameters.AddWithValue("@Explicit", editSong.Explicit);
                        int affectedRows = await command.ExecuteNonQueryAsync();
                        if (affectedRows > 0)
                        {
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
                errorMessage = "We experienced an error while adding to the database";
                Console.WriteLine("Error updating song in database: " + ex.Message);
            }
        }
    }
}
