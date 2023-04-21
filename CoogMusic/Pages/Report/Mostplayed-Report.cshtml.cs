
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
    public class Mostplayed_ReportModel : PageModel
    {
        public List<AlbumInfo> Albums = new List<AlbumInfo>();
        public int SelectedAlbumId { get; set; }
        public string SelectedAlbumTitle { get; set; }
        public int ArtistID;
        public string ReportHtml { get; set; }
        private readonly DbHelper _dbHelper;
        private readonly string connectionStr;
        public string ReportTitle { get; set; }


        public Mostplayed_ReportModel(IConfiguration configuration)
        {

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _dbHelper = new DbHelper(connectionString);
           
        }

        public async Task OnGetAsync()
        {
            ArtistID = await _dbHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            using (var connection = new MySqlConnection(connectionStr))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT id,artist_id,title FROM album WHERE artist_id = @ArtistId", connection))
                {
                    command.Parameters.AddWithValue("@ArtistId", ArtistID);


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

        public async Task<string> GenerateMostplayedReport(int AlbumId)
        {

            // saves artists userId in the variable  
            ArtistID = _dbHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))).Result;
            SelectedAlbumId = AlbumId;
            using (var connection = new MySqlConnection(connectionStr))
            using (var command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT s.title, COUNT(l.song_id) AS play_count FROM album_song AS a JOIN song AS s ON a.song_id = s.id JOIN listening_history AS l ON a.song_id = l.song_id WHERE a.album_id = @selectedAlbumId GROUP BY s.id, s.title ORDER BY play_count DESC LIMIT 20";
                command.Parameters.AddWithValue("@selectedAlbumId", SelectedAlbumId);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    StringBuilder html = new StringBuilder();

                    List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
                    html.Append("<table>");
                    html.Append("<tr><th>Song Title</th><th class='times-played'>Times Played</th></tr>");
                    html.Append("<style>.times-played { padding-left: 200px; } </style>");

                    while (reader.Read())
                    {
                        Dictionary<string, string> item1 = new Dictionary<string, string>();

                        var songTitle = reader.GetString("title");
                        var timesPlayed = reader.GetInt32("play_count");
                        item1.Add("title", songTitle);
                        item1.Add("played", timesPlayed.ToString());
                        data.Add(item1);
                    }

                    foreach (Dictionary<string, string> item in data)
                    {
                        string title = item["title"];
                        string played = item["played"];

                        // Create a new row 
                        string newRow = "<tr><td>" + title + "</td><td class='times-played'>" + played + "</td></tr>";
                        html.Append(newRow);
                    }

                    html.Append("</table>");
                    ReportHtml=html.ToString();


                    return ReportHtml;
                }
            }
        }
        public async Task<IActionResult> OnPostGenerateReportAsync(int AlbumId)
        {
            await GenerateMostplayedReport(AlbumId);
            return Page();
        }


    }
}







