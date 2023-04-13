using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.History
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<Historyofuser> listSongs = new List<Historyofuser>();
        public void OnGet()
        {
            try
            {
                String connectionStr = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    connection.Open();
                   
                    // Retrieve the top 5 most recent songs played by the user
                    String selectSql = "SELECT lh.play_time, s.title, s.genre, a.name " +
                                       "FROM listening_history lh " +
                                       "JOIN song s ON lh.song_id = s.id " +
                                       "JOIN artist a ON s.artist_id = a.artist_id " +
                                       "JOIN album al ON s.artist_id = al.artist_id " +
                                       "WHERE lh.user_id = @UserId " +
                                       "ORDER BY lh.play_time DESC " +
                                       "LIMIT 5;";
                    using (MySqlCommand command = new MySqlCommand(selectSql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Historyofuser historyofuser = new Historyofuser();
                                historyofuser.title = reader.GetString ("title");
                                historyofuser.genre = reader.GetString ("genre");
                                historyofuser.artist = reader.GetString("name");
                                historyofuser.last_played = reader.GetDateTime("play_time").ToString("MM/dd/yyyy hh:mm:ss");
                                listSongs.Add(historyofuser);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception" + ex.ToString());
            }
        }


    }
}
