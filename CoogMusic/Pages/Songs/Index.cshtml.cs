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

namespace CoogMusic.Pages.Upload
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
                    String sql = "SELECT * FROM song AS S, artist AS A WHERE A.artist_id=S.artist_id";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongInfo songInfo = new SongInfo();
                                songInfo.Name = reader.GetString(2);
                                songInfo.RecordLabel = reader.GetString(4);
                                songInfo.CreateDate = reader.GetDateTime(5).ToString();

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
        public void OnPost()
        {
            /*try
            {
                string connectionString = "server=coogmusic.mysql.database.azure.com;port=3306;user=qalksktvpv@coogmusic.mysql.database.azure.com;password=coogmusic1!;database=coogmusicDB;sslmode=Preferred;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    //string query = "INSERT INTO Songs (Title, Artist, Genre, Filename) VALUES (@title, @artist, @genre, @filename)";
                    string query = "Create table nothing (id INT NOT NULL PRIMARY KEY, name varchar(30));";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    //command.Parameters.AddWithValue("@title", Request.Form["title"]);
                    //command.Parameters.AddWithValue("@artist", Request.Form["artist"]);
                    //command.Parameters.AddWithValue("@genre", Request.Form["genre"]);
                    //command.Parameters.AddWithValue("@filename", Request.Form["file"]);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Log the exception here or display an error message to the user
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }*/
        }
        public class SongInfo
        {
            public int ArtistId;
            public int UserId;
            public String Name;
            public String Description;
            public String RecordLabel;
            public String CreateDate;
            public int SongId;
            //public int ArtistId;
            //public string Title;
            //public string Genre;
            //public DateTime UploadDate;
            //public TimeSpan Duration;
            //public int Likes;
            //public byte[] Track;
        }
    }
}
