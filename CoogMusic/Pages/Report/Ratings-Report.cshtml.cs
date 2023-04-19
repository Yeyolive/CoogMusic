using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CoogMusic.Data;
using System.Security.Claims;
using System.Text;

namespace CoogMusic.Pages.Report
{
    public class Ratings_ReportModel : PageModel
    {
        public List<AlbumInfo> Albums { get; set; }
        [BindProperty]
        public int SelectedAlbumId { get; set; }
        public string SelectedAlbumTitle { get; set; }
        public int ArtistId;
        public string ReportHtml { get; set; }
        private readonly DbHelper _dbHelper;
        private readonly string connectionStr;

        
        public Ratings_ReportModel(IConfiguration configuration)
        {

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _dbHelper = new DbHelper(connectionString);
        }

        public async Task OnGetAsync()
        {
            ArtistId = await _dbHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            using (var connection = new MySqlConnection(connectionStr))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT id,artist_id,title FROM album WHERE artist_id = @ArtistId", connection))
                {
                    command.Parameters.AddWithValue("@ArtistId", ArtistId);


                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Albums = new List<AlbumInfo>();
                        while (await reader.ReadAsync())
                        {
                            Albums.Add(new AlbumInfo
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                ArtistId = reader.GetInt32(reader.GetOrdinal("artist_id"))

                            });

                        }
                    }
                }

            }
        }

        public async Task<string> GenerateRatingReport(int selectedAlbumId)
        {


            using (var connection = new MySqlConnection(connectionStr))
            using (var command = new MySqlCommand())
            {
                SelectedAlbumId = selectedAlbumId;
                command.Connection = connection;
                command.CommandText = "SELECT song.title, song_rating.rating FROM song_rating JOIN album_song ON song_rating.song_id = album_song.song_id JOIN song ON song.id = song_rating.song_id WHERE album_song.album_id=@albumId";
                command.Parameters.AddWithValue("@albumId", SelectedAlbumId);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    StringBuilder html = new StringBuilder();
                    html.Append("<table");
                    html.Append("<tr><th>Song Title</th><th>Rating</th></tr>");
                    double totalAlbumRating = 0;

                    while (reader.Read())
                    {

                        var songTitle = reader.GetString("title");
                        var rating = reader.GetInt32("rating");
                        totalAlbumRating += rating;

                        html.Append("<tr>");
                        html.Append("<td>" + songTitle + "</td>");
                        html.Append("<td>" + rating.ToString() + "</td>");
                        html.Append("</tr>");
                    }
                    html.Append("<tr><td>Total album rating:</td><td>" + totalAlbumRating.ToString() + "</td></tr>");

                    html.Append("</table>");

                    await OnGetAsync();
                    SelectedAlbumTitle = Albums.FirstOrDefault(a => a.Id == selectedAlbumId)?.Title ?? "";
                    ReportHtml = html.ToString();

                    return ReportHtml;


                }
            }

        }

        public async Task<IActionResult> OnPostGenerateReportAsync(int selectedAlbumId)
        {
            await GenerateRatingReport(selectedAlbumId);
            return Page();
        }


    }
}