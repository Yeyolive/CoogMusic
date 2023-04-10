using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoogMusic.Pages.Search
{
    public class AddToPlaylistModel : PageModel
    {
        private readonly DbHelper _dbHelper; // Replace CoogMusicDbContext with your actual DbContext class name

        public AddToPlaylistModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbHelper = new DbHelper(connectionString);
        }

        [BindProperty(SupportsGet = true)]
        public int SongId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PlaylistId { get; set; }

        public bool Success { get; set; }

        //public async Task<IActionResult> OnPostAddSongToPlaylist(PlaylistInfo viewModel)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }

        //    var connectionString = _configuration.GetConnectionString("DefaultConnection");

        //    using (var connection = new MySqlConnection(connectionString))
        //    {
        //        var query = "INSERT INTO playlist_songs (playlist_id, song_id, song_title, artist_name) VALUES (@PlaylistId, @SongId, @SongTitle, @ArtistName)";
        //        var affectedRows = await connection.ExecuteAsync(query, viewModel);

        //        if (affectedRows > 0)
        //        {
        //            return Ok();
        //        }
        //        else
        //        {
        //            return StatusCode(500);
        //        }
        //    }
        //}
    }
}
