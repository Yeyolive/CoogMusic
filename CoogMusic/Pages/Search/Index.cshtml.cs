using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.Search
{
    public class IndexModel : PageModel
    {
        public List<SongInfo> listSongs = new List<SongInfo>();
        public void OnGet()
        {
            try
            {
                //using (MySqlConnection connection = new MySqlConnection(connectionString))
                //{
                //    connection.Open();
                //    String sql = "SELECT * FROM songs";
                //    using (MySqlCommand command = new MySqlConnection(sql, connection))
                //    {
                //        SongInfo songInfo = new SongInfo();
                //        songInfo.title = reader.GetString(1);
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
        }
        public class SongInfo
        {
            public String title;
            public String artist;
            public String album;
        }
    }
}
