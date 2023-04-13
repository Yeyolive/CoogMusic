using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.Report
{

    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<SongInfo> listSongs = new List<SongInfo>();
        public List<AlbumInfo> listAlbums = new List<AlbumInfo>();

        public void OnGet()
        {


            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                String connectionStr = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();
                    String sql = "SELECT s.id, s.artist_id, s.album_id, s.title, s.deleted FROM song AS s JOIN artist AS a ON s.artist_id=a.artist_id WHERE a.user_id=@UserId ORDER BY s.title";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongInfo songInfo = new SongInfo();
                                songInfo.songId = reader.GetInt32("id");
                                songInfo.artistId = reader.GetInt32("artist_id");
                                songInfo.title = reader.GetString("title");
                                songInfo.deleted = reader.GetBoolean("deleted");

                                AlbumInfo albumInfo = new AlbumInfo();
                                albumInfo.AlbumId = reader.GetInt32("album_id");
                                listSongs.Add(songInfo);
                                listAlbums.Add(albumInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception" + ex.ToString());
            }
        }


    }
}