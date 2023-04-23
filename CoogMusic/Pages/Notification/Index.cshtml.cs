using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Data;
using Newtonsoft.Json;


namespace CoogMusic.Pages.Notification
{
	public class IndexModel : PageModel
    {
        public String errorMessage = "";
        public String successMessage = "";
        public SongInfo songInfo = new SongInfo();

        private readonly DbHelper _databaseHelper;
        // Access this by calling connectionStr variable in your functions (GLOBAL)
        private readonly string connectionStr;
        public int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public List<NotificationData> notifications = new List<NotificationData>();

        public IndexModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _databaseHelper = new DbHelper(connectionString);
        }

        public async void OnGetAsync()
        {
            int artistId = await _databaseHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));

            using (MySqlConnection connection = new MySqlConnection(connectionStr))
            {
                connection.Open();
                string sql = "SELECT message FROM notification WHERE artist_id = @ArtistId";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ArtistId", artistId);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NotificationData notification = new NotificationData();
                            notification.Message = reader["message"].ToString();
                            Console.WriteLine(notification.Message);
                            notifications.Add(notification);
                        }
                    }
                }
            }
        }




        public class NotificationData
        {
            public string? Message { get; set; }
        }
    }
}
