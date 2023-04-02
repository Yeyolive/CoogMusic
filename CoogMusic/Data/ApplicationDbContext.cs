using CoogMusic.Pages;
using Microsoft.EntityFrameworkCore;

namespace CoogMusic.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Listener>()
                .HasOne(l => l.User)
                .WithOne(u => u.Listener)
                .HasForeignKey<Listener>(l => l.Id)
                .IsRequired();

            modelBuilder.Entity<Artist>()
                .HasOne(a => a.User)
                .WithOne(u => u.Artist)
                .HasForeignKey<Artist>(a => a.UserId)
                .IsRequired();
        }
    }

}