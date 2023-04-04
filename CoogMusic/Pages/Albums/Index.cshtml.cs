using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoogMusic.Pages.Albums
{

	public class IndexModel : PageModel
    {
        public List<AlbumInfo> albumInfo = new List<AlbumInfo>();

        public void OnGet()
        {
        }
    }
}
