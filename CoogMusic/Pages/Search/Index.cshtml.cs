using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoogMusic.Pages.Songs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Data;
using Newtonsoft.Json;

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
                    String sql = "SELECT s.id, s.artist_id, s.title, s.genre, s.track, s.upload_date, s.deleted, s.explicit, s.duration, a.name FROM song AS s JOIN artist AS a ON s.artist_id=a.artist_id WHERE s.title LIKE @SearchTerm OR a.name LIKE @SearchTerm;";
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

                                if (songInfo.deleted != true)
                                {
                                    listSongs.Add(songInfo);
                                }
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

        [BindProperty(SupportsGet = true)]
        public int rating { get; set; }

        public IActionResult OnPostUpdateRating()
        {
            int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            String connectionStr = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM song_rating WHERE listener_id = @ListenerID AND song_id = @SongID", connection))
                    {
                        command.Parameters.AddWithValue("@ListenerID", userID);
                        command.Parameters.AddWithValue("@songID", SongId);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            using (MySqlCommand comm = new MySqlCommand("UPDATE song_rating SET rating = @Rating WHERE listener_id = @ListenerID AND song_id = @SongID", connection))
                            {
                                comm.Parameters.AddWithValue("@Rating", rating);
                                comm.Parameters.AddWithValue("@ListenerID", userID);
                                comm.Parameters.AddWithValue("@SongID", SongId);
                                comm.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (MySqlCommand comm = new MySqlCommand("INSERT INTO song_rating (listener_id, song_id, rating)  VALUES (@ListenerID, @SongID, @Rating)", connection))
                            {
                                comm.Parameters.AddWithValue("@ListenerID", userID);
                                comm.Parameters.AddWithValue("@SongID", SongId);
                                comm.Parameters.AddWithValue("@Rating", rating);
                                comm.ExecuteNonQuery();
                            }
                        }
                    }
                }
                return new JsonResult(new { success = true, message = "Rating updated successfully" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error updating rating: {ex.Message}" });
            }
        }

        [BindProperty(SupportsGet = true)]
        public int SongId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PlaylistId { get; set; }

        public bool Success { get; set; }

        [BindProperty]
        public PlaylistSongInfo PlaylistSongData { get; set; }
        public async Task<IActionResult> OnGetUserPlaylists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var playlists = new List<PlaylistView>();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT p.id, p.user_id, p.title, p.description, p.deleted FROM playlist AS p WHERE user_id = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var title = reader.GetString("title");

                            if (reader.GetBoolean("deleted") != true)
                            {
                                playlists.Add(new PlaylistView
                                {
                                    Id = reader.GetInt32("id"),
                                    UserId = reader.GetInt32("user_id"),
                                    Title = reader.GetString("title"),
                                    Description = !reader.IsDBNull(reader.GetOrdinal("description")) ? reader.GetString("description") : null,
                                });
                            }
                        }
                    }
                }
            }
            //return new JsonResult(playlists);
            return Content(JsonConvert.SerializeObject(playlists), "application/json");
        }

        public async Task<IActionResult> OnPostAddToPlaylistAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string errorMessage = string.Join(", ", errors);
                return new JsonResult(new { success = false, message = "Invalid model state: " + errorMessage });
            }

            String connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlTransaction mySqlTransaction = connection.BeginTransaction();

                String sql = "INSERT INTO playlist_song (playlist_id, song_id) VALUES (@PlaylistId, @SongId);";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Transaction = mySqlTransaction;

                    command.Parameters.AddWithValue("@PlaylistId", PlaylistSongData.PlaylistId);
                    command.Parameters.AddWithValue("@SongId", PlaylistSongData.SongId);
                    int affectedRows = await command.ExecuteNonQueryAsync();

                    if (affectedRows > 0)
                    {
                        mySqlTransaction.Commit();
                        return new JsonResult(new { success = true });
                    }
                    else
                    {
                        return new JsonResult(new { success = false });
                    }
                }
            }
        }

        public class PlaylistView
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("userId")]
            public int UserId { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }
        }
    }
}
