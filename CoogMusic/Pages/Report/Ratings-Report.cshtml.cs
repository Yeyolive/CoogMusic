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
using System.Threading.Tasks;

namespace CoogMusic.Pages.Report
{
    public class Ratings_ReportModel : PageModel
    {
        public List<AlbumInfo> Albums { get; set; }
        public List<SongInfo> Songs { get; set; }

        [BindProperty]
        public int SelectedAlbumId { get; set; }
        public int SelectedSongId { get; set; }
        public string SelectedAlbumTitle { get; set; }
        public string SelectedSongTitle { get; set; }
        public int ArtistId;
        public string ReportHtml { get; set; }
        private readonly DbHelper _dbHelper;
        private readonly string connectionStr;



        public Ratings_ReportModel(IConfiguration configuration)
        {

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _dbHelper = new DbHelper(connectionString);
            SelectedAlbumId = 0;
            SelectedSongId = 0; 

        }

        public async Task OnGetAsync()
        {
            ArtistId = await _dbHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            using (var connection = new MySqlConnection(connectionStr))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT id,artist_id,title FROM album WHERE artist_id = @ArtistId ORDER BY title ASC", connection))
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
            using (var connection = new MySqlConnection(connectionStr))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT id,artist_id,title FROM song WHERE artist_id = @ArtistId ORDER BY title ASC", connection))
                {
                    command.Parameters.AddWithValue("@ArtistId", ArtistId);


                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Songs = new List<SongInfo>();
                        while (await reader.ReadAsync())
                        {
                            Songs.Add(new SongInfo
                            {
                                songId = reader.GetInt32(reader.GetOrdinal("id")),
                                title = reader.GetString(reader.GetOrdinal("title")),
                                artistId = reader.GetInt32(reader.GetOrdinal("artist_id"))

                            });

                        }
                    }
                }

            }
        }

        public async Task<string> GenerateRatingReport(int SelectedSongId,int SelectedAlbumId)
        {
            this.SelectedAlbumId = SelectedAlbumId;
            this.SelectedSongId = SelectedSongId;
            if (SelectedSongId == 0) { 
            using (var connection = new MySqlConnection(connectionStr))
            using (var command = new MySqlCommand())
            {
                
                command.Connection = connection;
                command.CommandText = "SELECT  song.title, IFNULL(AVG(song_rating.rating), 0) AS avg_rating, COUNT(DISTINCT song_rating.user_id) AS num_listeners_rated FROM song JOIN album_song ON song.id = album_song.song_id LEFT JOIN song_rating ON song.id = song_rating.song_id WHERE album_song.album_id = @albumId GROUP BY song.id, song.title";
                command.Parameters.AddWithValue("@albumId", SelectedAlbumId);
                connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        StringBuilder html = new StringBuilder();
                        List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
                        html.Append("<table>");
                        html.Append("<tr><th>Song Title</th><th class='rating-column'>Rating</th><th class='listener-column'>Number of listeners rated</th></tr>");
                        html.Append("<style>.rating-column { padding-left: 200px; } .listener-column { padding-left: 300px; }</style>");

                        float totalAlbumRating = 0;
                        int count = 0;
                        bool empty = true;

                        while (reader.Read())
                        {
                            empty = false;
                            Dictionary<string, string> item1 = new Dictionary<string, string>();
                            var songTitle = reader.GetString("title");
                            var rating = reader.GetFloat("avg_rating");
                            var listenerCount = reader.GetInt32("num_listeners_rated");
                            item1.Add("title", songTitle);
                            if (rating == 0)
                            {
                                item1.Add("rating", "No ratings yet");
                            }
                            else
                            {
                                item1.Add("rating", rating.ToString());
                            }

                            item1.Add("listeners", listenerCount.ToString());
                            data.Add(item1);

                            totalAlbumRating += rating;
                            if (rating > 0)
                            {
                                count++;
                            }

                        }
                        foreach (Dictionary<string, string> item in data)
                        {
                            // Get the title, rating, and listeners values from the current item
                            string title = item["title"];
                            string rating = item["rating"];
                            string listeners = item["listeners"];

                            // Create a new row with the title, rating, and listeners values
                            string newRow = "<tr><td>" + title + "</td><td class='rating-column'>" + rating + "</td><td class='listener-column'>" + listeners + "</td></tr>";
                            html.Append(newRow);
                        }


                        string Row = "<tr><td>" + "" + "</td><td class='rating-column'>" + "" + "</td><td class='listener-column'>" + "" + "</td></tr>";
                        if (empty == false)
                        {
                            totalAlbumRating = totalAlbumRating / count;
                            html.Append(Row); // Add empty rows
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);

                            html.Append("<tr><td>Total album rating:</td><td>" + totalAlbumRating.ToString() + "</td></tr>");
                        }
                        else
                        {
                            html.Append("<tr><td>No songs in this album.</td><td></td></tr>");
                        }

                        html.Append("</table>");

                        await OnGetAsync();

                        ReportHtml = html.ToString();

                        return ReportHtml;
                    }

                }
            }
            else
            {
                using (var connection = new MySqlConnection(connectionStr))
                using (var command = new MySqlCommand())
                {

                    command.Connection = connection;
                    command.CommandText = "SELECT song.title, IFNULL(AVG(song_rating.rating), 0) AS avg_rating, COUNT(DISTINCT song_rating.user_id) AS num_listeners FROM song LEFT JOIN song_rating ON song.id = song_rating.song_id AND song.artist_id = song_rating.artist_id WHERE song.id = @songId GROUP BY song.id, song.title ";
                    command.Parameters.AddWithValue("@songId", SelectedSongId);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        StringBuilder html = new StringBuilder();
                        List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
                        html.Append("<table>");
                        html.Append("<tr><th>Song Title</th><th class='rating-column'>Rating</th><th class='listener-column'>Number of listeners rated</th></tr>");
                        html.Append("<style>.rating-column { padding-left: 200px; } .listener-column { padding-left: 300px; }</style>");



                        while (reader.Read())
                        {
                            
                            Dictionary<string, string> item1 = new Dictionary<string, string>();
                            var songTitle = reader.GetString("title");
                            var rating = reader.GetFloat("avg_rating");
                            var listenerCount = reader.GetInt32("num_listeners");
                            item1.Add("title", songTitle);
                            if (rating == 0)
                            {
                                item1.Add("rating", "No ratings yet");
                            }
                            else
                            {
                                item1.Add("rating", rating.ToString());
                            }

                            item1.Add("listeners", listenerCount.ToString());
                            data.Add(item1);

                        }
                        foreach (Dictionary<string, string> item in data)
                        {
                            // Get the title, rating, and listeners values from the current item
                            string title = item["title"];
                            string rating = item["rating"];
                            string listeners = item["listeners"];

                            // Create a new row with the title, rating, and listeners values
                            string newRow = "<tr><td>" + title + "</td><td class='rating-column'>" + rating + "</td><td class='listener-column'>" + listeners + "</td></tr>";
                            html.Append(newRow);
                        }

                        html.Append("</table>");

                        await OnGetAsync();

                        ReportHtml = html.ToString();

                        return ReportHtml;
                    }

                }
            }

        }



        public async Task<IActionResult> OnPostGenerateReportAsync(int SelectedSongId, int SelectedAlbumId)
        {
            await GenerateRatingReport(SelectedSongId,SelectedAlbumId);
            return Page();
        }




    }
}