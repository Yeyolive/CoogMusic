using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CoogMusic.Data;

namespace CoogMusic.Pages.Report
{
    public class Listener_ReportModel : PageModel
    {
        public List<AlbumInfo> Albums { get; set; }
        public int SelectedAlbumId { get; set; }
        public string SelectedAlbumTitle { get; set; }
        public string ReportHtml { get; set; }

        private readonly IConfiguration _configuration;

        public Listener_ReportModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM Album", connection))
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

        public async Task<string> GenerateListenerReport(int selectedAlbumId)
        {
            if (selectedAlbumId == 0)
            {
                // handle the case where the album is null, e.g. by throwing an exception or returning a default value
                throw new ArgumentException("Album not found");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var connection = new MySqlConnection(connectionString))
            using (var command = new MySqlCommand())
            {
                SelectedAlbumId = selectedAlbumId;

                command.Connection = connection;
                command.CommandText = "SELECT song.title, COUNT(streams.song_id) AS stream_count FROM song JOIN album_song ON song.id = album_song.song_id JOIN album ON album_song.album_id = album.id JOIN artist ON album.artist_id = artist.artist_id LEFT JOIN streams ON song.id = streams.song_id WHERE album.id = @AlbumId GROUP BY song.id, song.title";

                command.Parameters.AddWithValue("@AlbumId", selectedAlbumId);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    StringBuilder html = new StringBuilder();
                    html.Append("<table>");
                    html.Append("<tr><th>Song Title</th><th>Streamed</th></tr>");

                    int totalAlbumStreams = 0;

                    while (reader.Read())
                    {
                        string songTitle = reader.GetString("title");
                        int streamCount = reader.GetInt32("stream_count");
                        totalAlbumStreams += streamCount;

                        html.Append("<tr>");
                        html.Append("<td>" + songTitle + "</td>");
                        html.Append("<td>" + streamCount.ToString() + "</td>");
                        html.Append("</tr>");
                    }

                    html.Append("<tr><td>Total album streams:</td><td>" + totalAlbumStreams.ToString() + "</td></tr>");

                    html.Append("</table>");

                    // Initialize Albums list before accessing it
                    await OnGetAsync();

                    SelectedAlbumTitle = Albums.FirstOrDefault(a => a.Id == selectedAlbumId)?.Title ?? "";
                    ReportHtml = html.ToString();

                    return ReportHtml;
                }
            }
        }


        public async Task<IActionResult> OnPostGenerateReportAsync(int selectedAlbumId)
        {
            await GenerateListenerReport(selectedAlbumId);
            return Page();
        }

    }



}
