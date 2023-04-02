using CoogMusic.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CoogMusic.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Listener>()
                .HasOne(l => l.User)
                .WithOne(u => u.Listener)
                .HasForeignKey<Listener>(l => l.UserId);

            modelBuilder.Entity<Artist>()
                .HasOne(a => a.User)
                .WithOne(u => u.Artist)
                .HasForeignKey<Artist>(a => a.UserId);
        }
    }

}