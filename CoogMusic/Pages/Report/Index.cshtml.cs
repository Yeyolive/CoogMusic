using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

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
                String connectionStr = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();
                    // Change this query for sorting
                    String sql = "SELECT s.id, s.artist_id, s.album_id s.title, s.deleted FROM song AS s JOIN artist AS a ON s.artist_id=a.artist_id WHERE a.user_id=@UserId ORDER BY s.title";
                    // Change this query to show only songs from the Artist that is logged in
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
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

        // When submit is pressed, this will activate
        //public Task OnPostAsync()
        //{

        //}
    }
}
