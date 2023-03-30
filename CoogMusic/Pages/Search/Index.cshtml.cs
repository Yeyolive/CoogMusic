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
        public void OnPost()
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
                        command.Parameters.AddWithValue("@SearchTerm", "%" + "NEED VALUE FROM SEARCH INPUT" + "%");
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongView songInfo = new SongView();
                                songInfo.title = reader.GetString(2);
                                songInfo.genre = reader.GetString(3);
                                songInfo.data = (byte[])reader["track"];

                                listSongs.Add(songInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public IActionResult PlaySong(int id)
        {
            // Retrieve the BLOB data for the song with the specified ID
            byte[] songData;
            String connectionStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
            using (MySqlConnection connection = new MySqlConnection(connectionStr))
            {
                connection.Open();
                String sql = "SELECT track FROM song WHERE id = @Id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            songData = (byte[])reader["track"];
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
            // Return the BLOB data as a file with the correct MIME type
            return File(songData, "audio/mpeg");
        }
    }
    public class SongView
    {
        public int songId;
        public String? title;
        public String? genre;
        public byte[]? data;
    }
}
