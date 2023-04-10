using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
namespace CoogMusic.Pages.Playlists
{
	public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<PlaylistInfo> UserPlaylists = new List<PlaylistInfo>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public String errorMessage = "";
        public String successMessage = "";

        public async Task OnGetAsync()
        {
            String connectionStr = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    await connection.OpenAsync();
                    MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                    String sql = "SELECT p.id, p.title, p.description, p.deleted FROM playlist AS p JOIN users AS u ON p.user_id=u.id WHERE p.user_id=@UserId ORDER BY p.title";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PlaylistInfo playlistInfo = new PlaylistInfo();
                                playlistInfo.Id = reader.GetInt32("id");
                                playlistInfo.Title = reader.GetString("title");
                                playlistInfo.Description = reader.GetString("description");
                                playlistInfo.Deleted = reader.GetBoolean("deleted");

                                if (playlistInfo.Deleted != true)
                                {
                                    UserPlaylists.Add(playlistInfo);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Retrieving Playlist From Database: " + ex.Message);
            }
        }
    }
}
