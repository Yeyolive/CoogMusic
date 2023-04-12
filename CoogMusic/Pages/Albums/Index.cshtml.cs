using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.Albums
{

	public class IndexModel : PageModel
    {
        private readonly DbHelper _databaseHelper;
        private readonly string connectionStr;

        public IndexModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _databaseHelper = new DbHelper(connectionString);
        }

        public List<AlbumInfo> albumInfo = new List<AlbumInfo>();

        public async Task OnGetAsync()
        {
            int artistId = await _databaseHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            Console.WriteLine(artistId.ToString());
            try
            {
                String connStr = connectionStr;
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    await connection.OpenAsync();
                    String getAlbumQuery = "SELECT a.id, a.title, a.description FROM album AS a JOIN artist AS r ON a.artist_id=r.artist_id WHERE r.artist_id=@UserID ORDER BY a.title";
                    using (MySqlCommand command = new MySqlCommand(getAlbumQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", artistId);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlbumInfo albumInfo = new AlbumInfo();
                                albumInfo.Id = reader.GetInt32("id");
                                //albumInfo.ArtistId = reader.GetInt32("artistid");
                                //albumInfo.art = reader.GetByte("art");
                                albumInfo.Title = reader.GetString("title");
                                albumInfo.Description = reader.GetString("description");
                                //albumInfo.ReleaseDate = reader.GetInt32("release_date");
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting album from database: " + ex.Message);
            }
        }
    }
}
