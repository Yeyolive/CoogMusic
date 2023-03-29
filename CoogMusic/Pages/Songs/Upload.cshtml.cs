using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Humanizer.Localisation;

namespace CoogMusic.Pages.Songs
{
	public class UploadModel : PageModel
    {
        public String errorMessage = "";
        public String successMessage = "";
        public SongInfo songInfo = new SongInfo();

        public async Task OnPostAsync()
        {
            songInfo.title = Request.Form["Title"];
            songInfo.artistId = int.Parse(Request.Form["Artist"]);
            songInfo.genre = Request.Form["Genre"];
            songInfo.songFile = Request.Form.Files["songfile"];
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
                    String sql = "INSERT INTO song (artist_id, title, genre, upload_date, track) VALUES (@ArtistId, @Title, @Genre, @UploadDate, @Track);";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Transaction = mySqlTransaction;

                        command.Parameters.AddWithValue("@ArtistId", songInfo.artistId);
                        command.Parameters.AddWithValue("@Title", songInfo.title);
                        command.Parameters.AddWithValue("@Genre", songInfo.genre);
                        command.Parameters.AddWithValue("@UploadDate", DateTime.UtcNow); 
                        command.Parameters.Add("@Track", MySqlDbType.Blob).Value = songData;

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
                errorMessage = "We experienced an error while adding to the database";
            }
            songInfo.title = ""; songInfo.genre = ""; songInfo.artistId = null; songInfo.songFile = null;
        }
    }
}
