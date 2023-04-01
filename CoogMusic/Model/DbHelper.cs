using System;
using MySql.Data.MySqlClient;
using System.Data;
using Azure;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CoogMusic.Pages
{
	public class DbHelper
    {
        String connectionString = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";
        public async Task CreateUser(ApplicationUser user, String password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                MySqlTransaction mySqlTransaction = conn.BeginTransaction();
                String sql = "INSERT INTO login (email, passwrd) VALUES (@Email, @Password)";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    
                    await cmd.ExecuteNonQueryAsync();
                }

                sql = "INSERT INTO users (name, email, mobile, create_date, sex, age) VALUES (@Name, @Email, @Mobile, @CreateDate, @Sex, @Age)";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Mobile", user.Mobile);
                    cmd.Parameters.AddWithValue("@CreateDate", user.CreateDate);
                    cmd.Parameters.AddWithValue("@Sex", user.Sex);
                    cmd.Parameters.AddWithValue("@Age", user.Age);
                    int affectedRows = await cmd.ExecuteNonQueryAsync();

                    await cmd.ExecuteNonQueryAsync();
                }
                mySqlTransaction.Commit();
            }
        }

        public async Task<ApplicationUser> GetUserByEmailAndPassword(String email, String pwd)
        {
            String connectionString = "Server=coogmusic.mysql.database.azure.com;User ID=qalksktvpv;Password=coogmusic1!;Database=coogmusicdb";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                String sql = "SELECT * FROM login AS l, users AS u WHERE l.email=u.email AND l.email=@Email AND l.passwrd=@Password";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", pwd);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ApplicationUser
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name"),
                                Email = reader.GetString("email"),
                                Mobile = reader.IsDBNull("mobile") ? null : reader.GetString("mobile"),
                                CreateDate = reader.GetDateTime("created_at"),
                                Sex = reader.GetChar("sex"),
                                Age = reader.GetInt32("age")
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}

