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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // ...
                // The rest of the method implementation
                // ...

                Success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Success = false;
            }

            return Page();
        }
    }
}
