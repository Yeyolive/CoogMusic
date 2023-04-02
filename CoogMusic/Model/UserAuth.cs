using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CoogMusic.Pages
{
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
        public string? Name { get; set; }
        //public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Mobile { get; set; }
        public string? recordLabel { get; set; }
        public DateTime CreateDate { get; set; }
        public char Sex { get; set; }
        public int Age { get; set; }

        public Listener? Listener { get; set; }
        public Artist? Artist { get; set; }
    }

    public class Login
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class Listener
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign Key
        public string? Name { get; set; }

        public ApplicationUser? User { get; set; }
    }

    public class Artist
    {
        public int ArtistId { get; set; }
        public int UserId { get; set; } // Foreign Key
        public string? Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string? Description { get; set; }
        public string? RecordLabel { get; set; }

        public ApplicationUser? User { get; set; }
    }
}

