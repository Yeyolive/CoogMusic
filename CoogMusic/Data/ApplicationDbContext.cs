using CoogMusic.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        public DbSet<Login> Logins { get; set; }
        public DbSet<Listener> Listeners { get; set; }
        public DbSet<Artist> Artists { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
            //base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseMySql(
            //            "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb",
            //    ServerVersion.AutoDetect("Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb"),
            //    options => options.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            //);
        //}

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<ApplicationUser>(entity =>
        //    {
        //        entity.Property(e => e.Email).HasColumnName("email");
        //        entity.ToTable("users");
        //    });

        //    builder.Entity<Listener>(entity =>
        //    {
        //        entity.HasKey(e => e.DbUserId);
        //        entity.ToTable("listener");
        //        entity.HasOne(e => e.User)
        //            .WithOne(u => u.Listener)
        //            .HasForeignKey<Listener>(e => e.DbUserId);
        //    });

        //    builder.Entity<Artist>(entity =>
        //    {
        //        entity.HasKey(e => e.ArtistId);
        //        entity.ToTable("artist");
        //        entity.HasOne(e => e.User)
        //            .WithOne(u => u.Artist)
        //            .HasForeignKey<Artist>(e => e.DbUserId);
        //    });

        //    builder.Entity<Login>(entity =>
        //    {
        //        entity.HasKey(e => e.Email);
        //        entity.ToTable("login");
        //        entity.Property(e => e.Password).HasColumnName("passwrd");
        //    });
        //}
    }
}