using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace CoogMusic.Pages.Playlists
{
	public class PlaylistSongsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<SongView> PlaylistSongs { get; set; }

        public PlaylistSongsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public int PlaylistId { get; set; }

        public List<SongView> playlistSongs = new List<SongView>();

        public async Task OnGetAsync()
        {
            int playlistId = PlaylistId;

            String connectionStr = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    await connection.OpenAsync();
                    MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                    String sql = @"
                                    SELECT
                                        s.id AS song_id,
                                        s.title AS song_title,
                                        s.genre AS song_genre,
                                        s.duration AS song_duration,
                                        s.deleted,
                                        s.explicit,
                                        a.artist_id AS artist_id,
                                        a.name AS artist_name
                                    FROM
                                        playlist_song AS ps
                                    JOIN song AS s ON
                                        ps.song_id = s.id
                                    JOIN artist AS a ON
                                        s.artist_id = a.artist_id
                                    WHERE
                                        ps.playlist_id = @PlaylistId AND
                                        ps.deleted = FALSE
                                    ORDER BY
                                        s.title
                                ";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PlaylistId", playlistId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {

                            while (await reader.ReadAsync())
                            {
                                SongView song = new SongView
                                {
                                    songId = reader.GetInt32("song_id"),
                                    artistId = reader.GetInt32("artist_id"),
                                    artistName = reader.GetString("artist_name"),
                                    title = reader.GetString("song_title"),
                                    genre = reader.IsDBNull("song_genre") ? null : reader.GetString("song_genre"),
                                    deleted = reader.GetBoolean("deleted"),
                                    Explicit = reader.GetBoolean("explicit"),
                                    Duration = TimeSpan.Parse(reader.GetString("song_duration")),
                                };
                                if (song.deleted != true)
                                {
                                    playlistSongs.Add(song);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Retrieving Playlist From Database: " + ex.Message);
            }
        }
    }
}
