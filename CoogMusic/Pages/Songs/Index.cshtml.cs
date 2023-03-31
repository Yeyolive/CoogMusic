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
                    String sql = "SELECT * FROM song AS s JOIN artist AS a ON s.artist_id=a.artist_id ORDER BY a.name";
                    // Change this query to show only songs from the Artist that is logged in
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongInfo songInfo = new SongInfo();
                                songInfo.songId = reader.GetInt32(0);
                                songInfo.title = reader.GetString(2);
                                songInfo.genre = reader.GetString(3);
                                songInfo.CreateDate = reader.GetDateTime(6).ToString();
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
    public class SongInfo
    {
        public String? RecordLabel;
        public String? CreateDate;
        public String? Name;
        public int? songId;
        public int? userId;
        public int? artistId;
        public String? artist;
        public String? genre;
        public String? title;
        public Boolean? deleted;
        public IFormFile? songFile;
    }
}
