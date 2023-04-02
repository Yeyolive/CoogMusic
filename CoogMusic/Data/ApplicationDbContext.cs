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
        public DbSet<Login> Logins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Other model configurations...

            modelBuilder.Entity<Listener>()
                .HasOne(l => l.User)
                .WithOne(u => u.Listener)
                .HasForeignKey<Listener>(l => l.Id); // Assuming Listener has a UserId foreign key property
        }

    }

}