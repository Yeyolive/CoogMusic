using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static CoogMusic.Pages.Edit.IndexModel;
using CoogMusic.Data;

namespace CoogMusic.Pages.Edit
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Song> Songs { get; set; }

        public class Song
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [ForeignKey("Artist")]
            public int ArtistId { get; set; }

            [Required]
            [StringLength(30)]
            public string Title { get; set; }

            [StringLength(30)]
            public string Genre { get; set; }

            [Required]
            public DateTime UploadDate { get; set; }

            [Required]
            public TimeSpan Duration { get; set; }

            public int Likes { get; set; }

            [Required]
            public byte[] Track { get; set; }

            public byte[] Lyrics { get; set; }

            public byte[] AlbumArt { get; set; }

            public string Artist { get; set; }
        }
    }
}
