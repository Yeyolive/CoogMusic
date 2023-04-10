using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CoogMusic.Pages.Playlists
{
	public class CreateModel : PageModel
    {
        private readonly DbHelper _databaseHelper;
        private readonly string connectionStr;

        public CreateModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _databaseHelper = new DbHelper(connectionString);
        }

        [BindProperty]
        public PlaylistInfo NewPlaylist { get; set; }

        public String successMessage = "";
        public String errorMessage = "";

        public async Task<IActionResult> OnPostAsync(String ListenerId, String Title, String Description)
        {
            // ... add code to insert a new playlist into the database ...
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    await connection.OpenAsync();
                    MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                    String sql = "INSERT INTO playlist (user_id, title, description) VALUES (@UserId, @Title, @Desc);";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Transaction = mySqlTransaction;

                        command.Parameters.AddWithValue("@UserId", ListenerId);
                        command.Parameters.AddWithValue("@Title", Title);
                        command.Parameters.AddWithValue("@Desc", Description);

                        int affectedRows = await command.ExecuteNonQueryAsync();
                        if (affectedRows > 0)
                        {
                            // Display a success message or redirect to another page
                            successMessage = "New Playlists Created";
                            Response.Redirect("/Playlists/");
                        }
                        else
                        {
                            errorMessage = "We experienced an error while adding to the database";
                        }
                        mySqlTransaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "We experienced an error while adding to the database";
                Console.WriteLine("Error inserting MP3 file into database: " + ex.Message);
            }
            return Page();
        }
    }
}
