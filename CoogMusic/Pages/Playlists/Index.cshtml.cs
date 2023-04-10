using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoogMusic.Model;

namespace CoogMusic.Pages.Playlists
{
	public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<PlaylistInfo> UserPlaylists { get; set; }

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            // ... add code to retrieve the user's playlists from the database ...
        }
    }
}
