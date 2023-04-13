using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CoogMusic.Data;

namespace CoogMusic.Pages.Report
{
    public class Ratings_ReportModel : PageModel
    {
        public List<AlbumInfo> Albums { get; set; }
        [BindProperty]
        public int SelectedAlbumId { get; set; }
        public string SelectedAlbumTitle { get; set; }
        public ReportData ReportData { get; set; }

        private readonly IConfiguration _configuration;

        public Ratings_ReportModel(IConfiguration configuration)
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
                            Title = reader.GetString(reader.GetOrdinal("title"))
                        });
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostGenerateReportAsync()
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand("SELECT * FROM Album WHERE Id=@id", connection))
                {
                    command.Parameters.AddWithValue("@id", SelectedAlbumId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                        {
                            return NotFound();
                        }

                        SelectedAlbumTitle = reader.GetString(reader.GetOrdinal("title"));
                    }
                }

                using (var command = new MySqlCommand("SELECT AVG(Rating) FROM SongRating WHERE SongId IN (SELECT Id FROM Song WHERE AlbumId=@albumId)", connection))
                {
                    command.Parameters.AddWithValue("@albumId", SelectedAlbumId);

                    var albumAverageRating = (await command.ExecuteScalarAsync() as double?) ?? 0;


                    using (var command2 = new MySqlCommand("SELECT * FROM Song WHERE AlbumId=@albumId", connection))
                    {
                        command2.Parameters.AddWithValue("@albumId", SelectedAlbumId);

                        using (var reader = await command2.ExecuteReaderAsync())
                        {
                            ReportData = new ReportData
                            {
                                AlbumAverageRating = albumAverageRating,
                                SongRatings = new List<SongRating>()
                            };

                            while (await reader.ReadAsync())
                            {
                                var songTitle = reader.GetString(reader.GetOrdinal("title"));
                                var songId = reader.GetInt32(reader.GetOrdinal("id"));

                                using (var command3 = new MySqlCommand("SELECT AVG(Rating) FROM SongRating WHERE SongId=@songId", connection))
                                {
                                    command3.Parameters.AddWithValue("@songId", songId);

                                    var songAverageRating = (double)(await command3.ExecuteScalarAsync() ?? 0);

                                    ReportData.SongRatings.Add(new SongRating
                                    {
                                        SongTitle = songTitle,
                                        Rating = songAverageRating
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return Page();
        }
    }



    public class SongRating
    {
        public string SongTitle { get; set; }
        public double Rating { get; set; }
        public double AverageRating { get; set; }

    }

    public class ReportData
    {
        public double AlbumAverageRating { get; set; }
        public List<SongRating> SongRatings
        {
            get; set;

        }
    }
}