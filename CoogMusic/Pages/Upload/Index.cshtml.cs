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
        public void OnGet()
        {
        }
        public IActionResult submitBtn_Click(object sender, EventArgs e, IFormatProvider file)
        {

            byte[] fileData;

            try
            {
                string connectionString = "Server=coogmusic.mysql.database.azure.com;User=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";

                /*using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO Songs (title, artist, genre, file_path) VALUES (@Title, @Artist, @Genre, @FilePath)";
                        command.Parameters.AddWithValue("@Title", Title);
                        command.Parameters.AddWithValue("@Genre", Genre);
                        command.Parameters.AddWithValue("@FilePath", FilePath);

                        command.ExecuteNonQuery();
                    }
                }*/

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                /*MessageBox.Show(ex.Message);*/
                return Page();
            }
        }
        public void OnPost()
        {
            /*try
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
            }*/
        }
    }
}
