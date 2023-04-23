using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CoogMusic.Data;
using System.Security.Claims;

namespace CoogMusic.Pages.Report
{
    public class Listener_ReportModel : PageModel
    {
        public List<AlbumInfo> Albums { get; set; }
        public List<SongInfo> Songs { get; set; }
        [BindProperty]
        public int SelectedAlbumId { get; set; }
        public int SelectedSongId { get; set; }
        public string SelectedSongTitle { get; set; }
        public string SelectedAlbumTitle { get; set; }
        public string ReportHtml { get; set; }
        public int ArtistId;
        private readonly DbHelper _dbHelper;
        private readonly string connectionStr;


        private readonly IConfiguration _configuration;

        public Listener_ReportModel(IConfiguration configuration)
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

        public async Task<string> GenerateListenerReport(int SelectedAlbumId, int SelectedSongId)
        {

            this.SelectedSongId = SelectedSongId;
            this.SelectedAlbumId = SelectedAlbumId;
            if (SelectedSongId == 0)
            { bool empty = true;

                using (var connection = new MySqlConnection(connectionStr))
                using (var command = new MySqlCommand())
                {


                    command.Connection = connection;
                    command.CommandText = "SELECT s.title AS Song_Title, COUNT(*) AS Times_Played,CASE WHEN song.deleted = 1 THEN 'Yes' ELSE 'No' END AS is_deleted FROM listening_history lh JOIN album_song als ON lh.song_id = als.song_id JOIN song s ON als.song_id = s.id AND als.album_id = @AlbumId LEFT JOIN song ON s.id = song.id GROUP BY s.id, s.title, is_deleted ORDER BY Song_Title ASC";
                    

                    command.Parameters.AddWithValue("@AlbumId", SelectedAlbumId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        StringBuilder html = new StringBuilder();
                        html.Append("<table>");
                        
                        html.Append("<tr><th>Song Title</th><th class='stream-column'>Streamed</th><th class='deleted-column'>Song Deleted</th></tr>");
                        html.Append("<style>.stream-column { padding-left: 200px; }.deleted-column { padding-left: 200px; } </style>");

                        int totalAlbumStreams = 0;

                        while (reader.Read())
                        {   empty = false;
                            string songTitle = reader.GetString("Song_Title");
                            int streamCount = reader.GetInt32("Times_Played");
                            var deleted = reader.GetString("is_deleted");
                            totalAlbumStreams += streamCount;
                            string newRow = "<tr><td>" + songTitle + "</td><td class='stream-column'>" + streamCount.ToString() + "</td><td class='deleted-column'>" + deleted + "</td></tr>";
                            html.Append(newRow);

                            
                        }
                        if (empty == false)
                        {
                            string Row = "<tr><td>" + "" + "</td><td class='rating-column'>" + "" + "</td><td class='listener-column'>" + "" + "</td><td class='deleted-column'>" + "" + "</td></tr>";
                            html.Append(Row); // Add empty rows
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);
                            html.Append(Row);

                            html.Append("<tr><td>Total album streams:</td><td>" + totalAlbumStreams.ToString() + "</td></tr>");
                        }
                        else
                        {
                            html.Append("<tr><td>No songs in this album.</td><td></td></tr>");
                        }

                        html.Append("</table>");

                        

                        

                        // Initialize Albums list before accessing it
                        await OnGetAsync();

                       // SelectedAlbumTitle = Albums.FirstOrDefault(a => a.Id == selectedAlbumId)?.Title ?? "";
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
                    command.CommandText = "SELECT s.title AS Song_Title, COUNT(*) AS Times_Played, CASE WHEN song.deleted = 1 THEN 'Yes' ELSE 'No' END AS is_deleted FROM listening_history lh JOIN song s ON lh.song_id = s.id AND s.id = @SongId LEFT JOIN song ON s.id = song.id GROUP BY s.id, s.title, is_deleted ORDER BY Song_Title ASC";


                    command.Parameters.AddWithValue("@SongId", SelectedSongId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        StringBuilder html = new StringBuilder();
                        html.Append("<table>");

                        html.Append("<tr><th>Song Title</th><th class='stream-column'>Streamed</th><th class='deleted-column'>Song Deleted</th></tr>");
                        html.Append("<style>.stream-column { padding-left: 200px; }.deleted-column { padding-left: 200px; } </style>");


                        bool notStreamed = true;
                        while (reader.Read())
                        {
                            notStreamed = false;
                            string songTitle = reader.GetString("Song_Title");
                            int streamCount = reader.GetInt32("Times_Played");
                            var deleted = reader.GetString("is_deleted");


                            string newRow = "<tr><td>" + songTitle + "</td><td class='stream-column'>" + streamCount.ToString() + "</td><td class='deleted-column'>" + deleted + "</td></tr>";
                            html.Append(newRow);

                        }
                        if (notStreamed == true) {
                            html.Append("<tr><td>Song Not Streamed.</td><td></td></tr>");
                        }
                        html.Append("</table>");


                        await OnGetAsync();

                        //SelectedAlbumTitle = Albums.FirstOrDefault(a => a.Id == selectedAlbumId)?.Title ?? "";
                        ReportHtml = html.ToString();

                        return ReportHtml;
                    }
                }
            }

        }



        public async Task<IActionResult> OnPostGenerateReportAsync(int SelectedAlbumId, int SelectedSongId)
        {
            await GenerateListenerReport(SelectedAlbumId, SelectedSongId);
            return Page();
        }
    }
}




