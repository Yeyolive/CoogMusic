using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoogMusic.Pages.Playlists
{
	public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public PlaylistInfo NewPlaylist { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // ... add code to insert a new playlist into the database ...
            return RedirectToPage("/MyPlaylists");
        }
    }
}
