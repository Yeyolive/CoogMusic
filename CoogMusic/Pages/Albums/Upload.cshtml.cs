using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Org.BouncyCastle.Utilities;
using Microsoft.AspNetCore.Identity;

namespace CoogMusic.Pages.Albums
{
    public class UploadModel : PageModel
    {
        public String errorMessage = "";
        public String successMessage = "";
        public AlbumInfo albumInfo = new AlbumInfo();

        private readonly IConfiguration _configuration;
        private readonly DbHelper _databaseHelper;
        private readonly string connStr;

        public UploadModel(IConfiguration configuration)
        {
            _configuration = configuration;
            connStr = _configuration.GetConnectionString("DefaultConnection");
            _databaseHelper = new DbHelper(connStr);
        }

        // Insert Album into database
        public async Task OnPostAsync(string albumTitle, string albumDesc, string genre, IFormFile albumArt)
        {
            albumInfo.ReleaseDate = Convert.ToInt32(Request.Form["AlbumYear"]);
            //albumInfo.ArtistId = await _databaseHelper.GetArtistIdByUserId(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            albumInfo.ArtistId = await _databaseHelper.GetArtistIdByUserId(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            Console.WriteLine("FINDING ERROR ::: " + albumInfo.ArtistId + " --- " + User.FindFirst(ClaimTypes.NameIdentifier).Value);
            byte[] albArt;

            using (var memoryStream = new MemoryStream())
            {
                await albumArt.CopyToAsync(memoryStream);
                //Bitmap image = (Bitmap)Image.FromStream(memoryStream);
                albArt = memoryStream.ToArray();
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    MySqlTransaction mySqlTransaction = conn.BeginTransaction();

                    // Insert album if it does not exist
                    /*String insertAlbumQuery = @"
                        INSERT INTO album (artist_id, title, description, art)
                        SELECT @ArtistId, @Title, @Description, @Art
                        WHERE NOT EXISTS (SELECT 1 FROM album WHERE artist_id=@ArtistId AND title=@Title);";
                    */
                    String insertAlbumQuery = "INSERT INTO album (artist_id, title, description, art) VALUES (@ArtistId, @Title, @Description, @Art)";
                    using (MySqlCommand insertAlbumCommand = new MySqlCommand(insertAlbumQuery, conn))
                    {
                        insertAlbumCommand.Transaction = mySqlTransaction;

                        insertAlbumCommand.Parameters.AddWithValue("@ArtistId", albumInfo.ArtistId);
                        insertAlbumCommand.Parameters.AddWithValue("@Title", albumTitle);
                        insertAlbumCommand.Parameters.AddWithValue("@Description", albumDesc);
                        insertAlbumCommand.Parameters.Add("@Art", MySqlDbType.Blob).Value = albArt;

                        int affectedRows = await insertAlbumCommand.ExecuteNonQueryAsync();
                        if (affectedRows > 0)
                        {
                            successMessage = "New Album Added Correctly";
                            Response.Redirect("/Albums/");
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
                errorMessage = "ERROR PUTTING ALBUM INTO DATABASE";
                Console.WriteLine("Error inserting album into database: " + ex.Message);
            }
        }
    }
}
