using CoogMusic.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;


    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddRazorPages();


// Ensure your app listens on port 8080
builder.WebHost.UseUrls("http://*:8080");

var app = builder.Build();

// Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }
    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseMigrationsEndPoint();
    //}
    //else
    //{
    //    app.UseExceptionHandler("/Error");
    //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //    app.UseHsts();
    //}

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapRazorPages();

    app.Run();
//namespace CoogMusic{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var host = new WebHostBuilder()
//              .UseKestrel()
//              .UseContentRoot(Directory.GetCurrentDirectory())
//              .UseIISIntegration()
//              .UseStartup<Startup>()
//              .Build();

//            host.Run();
//        }
//    }
//}