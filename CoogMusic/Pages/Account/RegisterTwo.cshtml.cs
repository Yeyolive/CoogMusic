using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoogMusic.Pages.Account
{
	public class RegisterTwoModel : PageModel
    {
        public String errorMessage = "";
        private readonly DbHelper _databaseHelper;

        public RegisterTwoModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _databaseHelper = new DbHelper(connectionString);
        }

        public async Task<IActionResult> OnPostAsync(String Name, String Email, String Mobile, String Password, Char Sex, int Age, String UserType)
        {
            // Check if the email is already registered
            bool emailExists = await _databaseHelper.EmailExists(Email);
            if (emailExists)
            {
                errorMessage = "Email Is Already In Use!";
                return Page();
            }
            else
            {
                ApplicationUser User = new ApplicationUser();
                User.Name = Name;
                User.Email = Email;
                User.Mobile = Mobile;
                User.Sex = Sex;
                User.Age = Age;

                await _databaseHelper.CreateLogin(User, Password, UserType);
                await _databaseHelper.CreateUser(User, UserType);
                await _databaseHelper.CreateArtistOrListener(User, UserType);

                return RedirectToPage("/Account/LoginTwo");
            }
        }
    }
}
