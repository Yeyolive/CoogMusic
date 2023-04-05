using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoogMusic.Pages.Songs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.Search
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<SongView> listSongs = new List<SongView>();
        public bool SearchPerformed { get; set; } = false;

        public void OnPost()
        {
            String? searched = Request.Form["Title"];
            try
            {
                String connectionStr = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open(); 
                    String sql = "SELECT s.*, a.name FROM song AS s JOIN artist AS a ON s.artist_id=a.artist_id WHERE s.title LIKE @SearchTerm OR a.name LIKE @SearchTerm;";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", "%" + searched + "%");

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SongView songInfo = new SongView();
                                songInfo.songId = reader.GetInt32("id");
                                songInfo.artistId = reader.GetInt32("artist_id");
                                songInfo.artistName = reader.GetString("name");
                                songInfo.title = reader.GetString("title");
                                songInfo.genre = reader.GetString("genre");
                                songInfo.trackBytes = (byte[])reader["track"];
                                songInfo.CreateDate = reader.GetDateTime("upload_date").ToString("MM/dd/yyyy");
                                songInfo.deleted = reader.GetBoolean("deleted");
                                songInfo.Explicit = reader.GetBoolean("explicit");
                                songInfo.Duration = TimeSpan.Parse(reader.GetString("duration"));

                                listSongs.Add(songInfo);
                            }
                        }
                    }
                }
                SearchPerformed = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public IActionResult OnGetPlaySong(int id)
        {
            // Retrieve the BLOB data for the song with the specified ID
            byte[] songData;
            String connectionStr = _configuration.GetConnectionString("DefaultConnection");
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
}
