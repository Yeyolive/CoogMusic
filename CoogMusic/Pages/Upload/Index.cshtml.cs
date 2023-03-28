using System.IO;
using System.Threading.Tasks;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace CoogMusic.Pages.Upload
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }
        [BindProperty]
        public string UploadType { get; set; }

        [BindProperty]
        public string Title { get; set; }

        [BindProperty]
        public string Artist { get; set; }

        [BindProperty]
        public string Genre { get; set; }
        [BindProperty]
        public string Path { get; set; }
        public void OnGet()
        {
        }
        public void OnPost()
        {
            try
            {
                string connectionString = "server=coogmusic.mysql.database.azure.com;port=3306;user=qalksktvpv@coogmusic.mysql.database.azure.com;password=coogmusic1!;database=coogmusicDB;sslmode=Preferred;";
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    //string query = "INSERT INTO Songs (Title, Artist, Genre, Filename) VALUES (@title, @artist, @genre, @filename)";
                    string query = "Create table nothing (id INT NOT NULL PRIMARY KEY, name varchar(30));";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    //command.Parameters.AddWithValue("@title", Request.Form["title"]);
                    //command.Parameters.AddWithValue("@artist", Request.Form["artist"]);
                    //command.Parameters.AddWithValue("@genre", Request.Form["genre"]);
                    //command.Parameters.AddWithValue("@filename", Request.Form["file"]);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Log the exception here or display an error message to the user
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
