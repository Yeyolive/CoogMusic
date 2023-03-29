using System.Security.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.Search
{
    public class IndexModel : PageModel
    {
        public List<SongView> listSongs = new List<SongView>();
        List<String> urls = new List<String>();
        public IActionResult Search(String q)
        {
            try
            {
                String connectionStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();
                    String sql = "SELECT * FROM song AS S WHERE name LIKE @song_searched";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", "%" + q + "%");
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongView songInfo = new SongView();
                                songInfo.title = reader.GetString(2);
                                songInfo.genre = reader.GetString(3);
                                songInfo.data = (byte[])reader["track"];

                                listSongs.Add(songInfo);
                                urls.Add($"data:audio/mp3;base64,{Convert.ToBase64String(songInfo.data)}");
                            }
                        }
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                return Page();
            }
        }
        public async Task OnPostAsync()
        {

        }
    }
    public class SongView
    {
        public String title;
        public String genre;
        public byte[] data;
    }
}
