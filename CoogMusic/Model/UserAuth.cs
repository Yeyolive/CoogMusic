using System;
using Microsoft.AspNetCore.Identity;

namespace CoogMusic.Pages
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Mobile { get; set; }
        public DateTime CreateDate { get; set; }
        public char Sex { get; set; }
        public int Age { get; set; }

        // Navigation properties for listener and artist
        public Listener? Listener { get; set; }
        public Artist? Artist { get; set; }
    }

    public class Listener
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        // Navigation property for user
        public ApplicationUser? User { get; set; }
    }

    public class Artist
    {
        public int ArtistId { get; set; }
        public string? Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Description { get; set; }
        public string? RecordLabel { get; set; }

        // Navigation property for user
        public ApplicationUser? User { get; set; }
    }
}

