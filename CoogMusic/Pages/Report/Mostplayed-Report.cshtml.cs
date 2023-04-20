
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
        public List<AlbumInfo> Albums { get; set; }
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
            ReportTitle = "Most Played Report";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // saves artists userId in the variable  
            ArtistID = _dbHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))).Result;

            using (var connection = new MySqlConnection(connectionStr))
            using (var command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT s.title AS Song_Title, COUNT(*) AS Times_Played FROM listening_history lh JOIN song s ON lh.song_id = s.id AND s.artist_id = @ArtistId GROUP BY s.id, s.title ORDER BY Times_Played DESC LIMIT 20";
                command.Parameters.AddWithValue("@ArtistId", ArtistID);
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

                        var songTitle = reader.GetString("Song_Title");
                        var timesPlayed = reader.GetInt32("Times_Played");
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


                    return Content(ReportHtml, "text/html");
                }
            }
        }


    }
}







