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
                    //String sql = "SELECT s.id, s.artist_id, s.title, s.genre, s.track, s.upload_date, s.deleted, s.explicit, s.duration, a.name FROM song AS s JOIN artist AS a ON s.artist_id=a.artist_id WHERE s.title LIKE @SearchTerm OR a.name LIKE @SearchTerm;";
                    String sql = @"
                                SELECT s.id, s.artist_id, s.title, s.genre, s.track, s.upload_date, s.deleted, s.explicit, s.duration, a.name,
                                (SELECT COUNT(*) FROM playlist_song AS ps WHERE ps.song_id = s.id AND ps.user_id = @UserId AND ps.deleted=false) > 0 AS is_in_playlist
                                FROM song AS s
                                JOIN artist AS a ON s.artist_id=a.artist_id
                                WHERE s.title LIKE @SearchTerm OR a.name LIKE @SearchTerm;
                                ";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
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

                                songInfo.IsInPlaylist = reader.GetBoolean("is_in_playlist");

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
            int? artistId;

            String connectionStr = _configuration.GetConnectionString("DefaultConnection");
            using (MySqlConnection connection = new MySqlConnection(connectionStr))
            {
                connection.Open();

                String sql = "SELECT track, artist_id FROM song WHERE id = @Id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            songData = (byte[])reader["track"];
                            artistId = reader["artist_id"] as int?;
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
            // Return the BLOB data as a file with the correct MIME type and artistId in the response header
            Response.Headers.Add("artist-id", artistId.ToString());
            return File(songData, "audio/mpeg");
        }

        [BindProperty(SupportsGet = true)]
        public int ArtistId { get; set; }

        public IActionResult OnPostFollowArtist()
        {
            int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var ArtistId = Request.Form["artistId"];
            int artistID = int.Parse(ArtistId);

            //Console.WriteLine("ARTIST ID: " + artistID);
            String connectionStr = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();

                    String sql = @"
                        INSERT INTO follows (listener_id, artist_id)
                        VALUES (@UserID, @ArtistID)
                        ON DUPLICATE KEY UPDATE
                        listener_id = IF(listener_id = @UserID, NULL, listener_id),
                        artist_id = IF(artist_id = @ArtistID, NULL, artist_id);
                    ";

                    using (MySqlCommand followArtist = new MySqlCommand(sql, connection))
                    {
                        followArtist.Parameters.AddWithValue("@UserID", userID);
                        followArtist.Parameters.AddWithValue("@ArtistID", artistID);
                        int rowsAffected = followArtist.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return new JsonResult(new { success = true, message = "Followed artist successfully" });
                        }
                        else
                        {
                            return new JsonResult(new { success = true, message = "Unfollowed artist successfully" });
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                return new JsonResult(new { success = false, message = $"Error following artist: {exc.Message}" });
            }
        }


        [BindProperty(SupportsGet = true)]
        public int rating { get; set; }
        public int artistid { get; set; }

        public IActionResult OnPostUpdateRating()
        {
            int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int Rating = Convert.ToInt32(Request.Form["rating"]);
            int SongId = Convert.ToInt32(Request.Form["songID"]);
            var artistID = Convert.ToInt32(Request.Form["artistid"]);
            int artistID = int.Parse(ArtistId);

            String connectionStr = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM song_rating WHERE user_id = @UserID AND song_id = @SongID", connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@SongID", SongId);

                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            using (MySqlCommand comm = new MySqlCommand("UPDATE song_rating SET rating = @Rating WHERE user_id = @UserID AND song_id = @SongID AND artist_id = @ArtistID", connection))
                            {
                                comm.Parameters.AddWithValue("@Rating", Rating);
                                comm.Parameters.AddWithValue("@UserID", userID);
                                comm.Parameters.AddWithValue("@SongID", SongId);
                                comm.Parameters.AddWithValue("@ArtistID", artistID);
                                comm.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            using (MySqlCommand comm = new MySqlCommand("INSERT INTO song_rating (user_id, song_id,artist_id, rating)  VALUES (@UserID, @SongID, @ArtistID, @Rating)", connection))
                            {
                                comm.Parameters.AddWithValue("@UserID", userID);
                                comm.Parameters.AddWithValue("@SongID", SongId);
                                comm.Parameters.AddWithValue("@Rating", Rating);
                                comm.Parameters.AddWithValue("@ArtistID", artistID);
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

                String checkSql = "SELECT COUNT(*) FROM playlist_song WHERE playlist_id = @PlaylistId AND song_id = @SongId AND deleted = FALSE;";
                using (MySqlCommand checkCommand = new MySqlCommand(checkSql, connection))
                {
                    checkCommand.Transaction = mySqlTransaction;

                    checkCommand.Parameters.AddWithValue("@PlaylistId", PlaylistSongData.PlaylistId);
                    checkCommand.Parameters.AddWithValue("@SongId", PlaylistSongData.SongId);
                    int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                    if (count > 0)
                    {
                        // Song is already in the playlist
                        return new JsonResult(new { success = false, message = "The song is already in the playlist."});
                    }
                }

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
