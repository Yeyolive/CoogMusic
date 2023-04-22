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


        public IndexModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _databaseHelper = new DbHelper(connectionString);
        }


        public List<NotificationMessage> GetNotifications(int UserId)
        {
            List<NotificationMessage> notifications = new List<NotificationMessage>();
            using (MySqlConnection connection = new MySqlConnection(connectionStr))
            {
                connection.Open();
                string sql = "SELECT * FROM notifications WHERE user_id = @userId";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userId", UserId);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NotificationMessage notification = new NotificationMessage();
                            notification.Message = reader["message"].ToString();
                            notifications.Add(notification);
                        }
                    }
                }
            }
            return notifications;
        }

        public class NotificationMessage
        {
            public string Message { get; set; }
        }
    }
}
