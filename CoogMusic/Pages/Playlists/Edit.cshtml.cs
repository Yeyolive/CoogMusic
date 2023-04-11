using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoogMusic.Pages.Playlists
{
	public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<SongView> PlaylistSongs { get; set; }

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public int PlaylistId { get; set; }

        public async Task OnGetAsync()
        {
            int playlistId = PlaylistId;

            String connectionStr = _configuration.GetConnectionString("DefaultConnection");

        }
    }
}
