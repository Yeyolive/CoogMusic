using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static CoogMusic.Pages.Edit.IndexModel;

namespace CoogMusic.Pages.Edit
{
	public class IndexModel : PageModel
    {
		public List<Songs> songInfo { get; set; }
		public void OnGet()
        {
        }
		public class Songs
		{
			[Key]
			[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
			public int id { get; set; }
			[Required(ErrorMessage = "Title is required")]
			public string title { get; set; }
			[Required(ErrorMessage = "Artist is required")]
			public string artist { get; set; }
			[Required(ErrorMessage = "Artist is required")]
			public string genre { get; set; }
			[Required(ErrorMessage = "Genre is required")]
			public string filename { get; set; }

		}
	}
}
