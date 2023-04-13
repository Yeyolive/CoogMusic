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

namespace CoogMusic.Pages.Albums
{
    public class AlbumSongsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<SongView> AlbumSongs { get; set; }

        public AlbumSongsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public int AlbumId { get; set; }

        public List<SongView> albumSongs = new List<SongView>();

        public async Task OnGetAsync()
        {
            int albumId = AlbumId;

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
                                        album_song
                                    JOIN song AS s ON
                                        album_song.song_id = s.id
                                    JOIN artist AS a ON
                                        s.artist_id = a.artist_id
                                    WHERE
                                        album_song.album_id = @AlbumId AND
                                        album_song.deleted = FALSE
                                    ORDER BY
                                        s.title
                                ";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AlbumId", albumId);

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
                                    albumSongs.Add(song);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Retrieving Album From Database: " + ex.Message);
            }
        }
    }
}
