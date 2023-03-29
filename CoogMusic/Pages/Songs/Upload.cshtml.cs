using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static CoogMusic.Pages.Search.IndexModel;
namespace CoogMusic.Pages.Upload
{
	public class UploadModel : PageModel
    {
        public SongInfo songInfo = new SongInfo();
        public void OnGet()
        {
        }
        public void OnPost()
        {
            songInfo.title = Request.Form["Title"];
            songInfo.artist = Request.Form["Artist"];
            songInfo.song_genre = Request.Form["Genre"];
            songInfo.title = Request.Form["Title"];

        }
    }
}
