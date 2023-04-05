using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CoogMusic.Pages.Albums
{
    public class UploadModel : PageModel
    {
        public String errorMessage = "";
        public String successMessage = "";
        // Insert Album into database
        public async Task OnPostAsync(int artistId, string albumTitle, string songTitle, string genre, IFormFile albumArt)
        {
            String connStr = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                await conn.OpenAsync();
                MySqlTransaction mySqlTransaction = conn.BeginTransaction();

                // Insert album if it does not exist
                string insertAlbumQuery = @"
                    INSERT INTO album (artist_id, title, description, art)
                    SELECT @ArtistId, @Title, @Description, @Art
                    WHERE NOT EXISTS (SELECT 1 FROM album WHERE artist_id=@ArtistId AND title=@Title);";

                using (MySqlCommand insertAlbumCommand = new MySqlCommand(insertAlbumQuery, conn))
                {
                    insertAlbumCommand.Transaction = mySqlTransaction;

                    insertAlbumCommand.Parameters.AddWithValue("@ArtistId", artistId);
                    insertAlbumCommand.Parameters.AddWithValue("@Title", albumTitle);

                    int affectedRows = await insertAlbumCommand.ExecuteNonQueryAsync();
                    if (affectedRows > 0)
                    {
                        successMessage = "New Song Added Correctly";
                        Response.Redirect("/Songs/");
                    }
                    else
                    {
                        errorMessage = "We experienced an error while adding to the database";
                    }

                    mySqlTransaction.Commit();
                }
            }
        }
    }
}
