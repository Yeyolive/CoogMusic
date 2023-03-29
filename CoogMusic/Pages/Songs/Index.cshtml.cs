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
                //String connectionStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
                //using (MySqlConnection connection = new MySqlConnection(connectionStr))
                //{
                //    connection.Open();
                //    String sql = "SELECT * FROM artist";
                //    using (MySqlCommand command = new MySqlCommand(sql, connection))
                //    {
                //        using (MySqlDataReader reader = command.ExecuteReader())
                //        {
                //            while (reader.Read())
                //            {
                //                SongInfo songInfo = new SongInfo();
                //                songInfo.Name = reader.GetString(2);
                //                songInfo.RecordLabel = reader.GetString(4);
                //                songInfo.CreateDate = reader.GetDateTime(5).ToString();

                //                listSongs.Add(songInfo);
                //            }
                //        }
                //    }
                //}
                String connectionStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();
                    String sql = "SELECT * FROM song AS S";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongInfo songInfo = new SongInfo();
                                songInfo.title = reader.GetString(2);
                                songInfo.genre = reader.GetString(3);
                                songInfo.CreateDate = reader.GetDateTime(6).ToString();

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
            public String RecordLabel;
            public String CreateDate;
            public String Name;
        public int songId;
            public int userId;
            public int? artistId;
            public String artist;
            public String genre;
            public String title;
            public IFormFile songFile;
            //public DateTime uploadDate;
            //public TimeSpan duration;
        }
}
