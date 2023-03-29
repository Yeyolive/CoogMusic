using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace CoogMusic.Pages.Songs
{
	public class UploadModel : PageModel
    {
        public String errorMessage = "";
        public String successMessage = "";
        public SongInfo songInfo = new SongInfo();
        public void OnGet()
        {
        }
        public void OnPost()
        {
            songInfo.title = Request.Form["Title"];
            songInfo.artistId = int.TryParse(Request.Form["Artist"], out int artistId) ? artistId : (int?)null;
            songInfo.genre = Request.Form["Genre"];
            songInfo.songFile = Request.Form.Files["songFile"];

            // Input data into database
            try
            {
                String connectionStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
                using (MySqlConnection connection = new MySqlConnection(connectionStr))
                {
                    MySqlTransaction mySqlTransaction = connection.BeginTransaction();
                    connection.Open();
                    String sql = "INSERT INTO songs " +
                                 "\"(artist_id, title, genre, upload_date, duration, likes, track) VALUES " +
                                 "(@artist_id, @title, @genre, @upload_date, @duration, @likes, @track);";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Transaction = mySqlTransaction;

                        command.Parameters.AddWithValue("@artist_id", songInfo.artistId);
                        command.Parameters.AddWithValue("@title", songInfo.title);
                        command.Parameters.AddWithValue("@genre", songInfo.genre);
                        command.Parameters.AddWithValue("@track", songInfo.songFile);
                        command.Parameters.AddWithValue("@upload_date", DateTime.Now.Date);
                        command.Parameters.AddWithValue("@duration", TimeSpan.Zero);
                        command.Parameters.AddWithValue("@likes", 0);
                        command.Parameters.AddWithValue("@lyrics", DBNull.Value);
                        command.ExecuteScalar();

                        command.ExecuteNonQuery();
                    }
                    mySqlTransaction.Commit();
                }
                successMessage = "New Song Added Correctly";
                Response.Redirect("/Songs/Index");
            }
            catch (Exception ex)
            {
                errorMessage = "We experienced an error while adding to the database";
            }
            songInfo.title = ""; songInfo.genre = ""; songInfo.artistId = null; songInfo.songFile = null;
        }
    }
}
