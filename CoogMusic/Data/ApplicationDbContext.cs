using CoogMusic.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CoogMusic.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Listener> Listeners { get; set; }
        public DbSet<Artist> Artists { get; set; }
    }
}