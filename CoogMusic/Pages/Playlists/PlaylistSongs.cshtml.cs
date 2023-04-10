using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task OnGetAsync(int playlistId)
        {
            // ... add code to retrieve the songs in the specified playlist from the database ...
        }
    }
}
