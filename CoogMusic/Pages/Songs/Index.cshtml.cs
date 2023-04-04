using System.IO;
using System.Threading.Tasks;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;

namespace CoogMusic.Pages.Songs
{
    public class IndexModel : PageModel
    {
        public List<SongInfo> listSongs = new List<SongInfo>();

        public void OnGet() {
            try
            {
                String connectionStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();
                    // Change this query for sorting
                    String sql = "SELECT * FROM song AS s JOIN artist AS a ON s.artist_id=a.artist_id WHERE a.user_id=@UserId ORDER BY s.title";
                    // Change this query to show only songs from the Artist that is logged in
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongInfo songInfo = new SongInfo();
                                songInfo.songId = reader.GetInt32(0);
                                songInfo.artistId = reader.GetInt32(1);
                                songInfo.title = reader.GetString(2);
                                songInfo.genre = reader.GetString(3);
                                songInfo.CreateDate = reader.GetDateTime(6).ToString("MM/dd/yyyy");
                                songInfo.deleted = reader.GetBoolean(7);

                                listSongs.Add(songInfo);
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
