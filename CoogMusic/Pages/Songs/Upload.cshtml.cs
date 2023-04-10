using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using Humanizer.Localisation;
using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CoogMusic.Pages.Songs
{
	public class UploadModel : PageModel
    {
        public String errorMessage = "";
        public String successMessage = "";
        public SongInfo songInfo = new SongInfo();

        private readonly DbHelper _databaseHelper;
        private readonly string connectionStr;

        public UploadModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _databaseHelper = new DbHelper(connectionString);
        }

        public List<AlbumInfo> listAlbums = new List<AlbumInfo>();

        public async void OnGetAsync()
        {
            songInfo.artistId = await _databaseHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));

        }

        [Authorize(Roles = "Artist")]
        public async Task OnPostAsync()
        {
            songInfo.title = Request.Form["Title"];
            songInfo.artistId = await _databaseHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            songInfo.genre = Request.Form["Genre"];
            songInfo.Explicit = Request.Form["Explicit"] == "true";
            songInfo.songFile = Request.Form.Files["songfile"];
            songInfo.Duration = TimeSpan.FromSeconds(Convert.ToDouble(Request.Form["Duration"]));
            byte[] songData;
            using (var memoryStream = new MemoryStream())
            {
                await songInfo.songFile.CopyToAsync(memoryStream);
                songData = memoryStream.ToArray();
            }
            // Input data into database
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    await connection.OpenAsync();
                    MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                    String sql = "INSERT INTO song (artist_id, title, genre, upload_date, explicit, duration, track) VALUES (@ArtistId, @Title, @Genre, @UploadDate, @Explicit, @Duration, @Track);";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Transaction = mySqlTransaction;

                        command.Parameters.AddWithValue("@ArtistId", songInfo.artistId);
                        command.Parameters.AddWithValue("@Title", songInfo.title);
                        command.Parameters.AddWithValue("@Genre", songInfo.genre);
                        command.Parameters.AddWithValue("@UploadDate", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@Duration", songInfo.Duration);
                        command.Parameters.AddWithValue("@Explicit", songInfo.Explicit);
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
                errorMessage = "We experienced an error while adding to the database";
                Console.WriteLine("Error inserting MP3 file into database: " + ex.Message);
            }
        }
    }
}
