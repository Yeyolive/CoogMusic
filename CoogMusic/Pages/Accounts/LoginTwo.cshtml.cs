using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;


namespace CoogMusic.Pages.Accounts
{
	public class LoginTwoModel : PageModel
    {
        private readonly DbHelper _databaseHelper;

        public LoginTwoModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _databaseHelper = new DbHelper(connectionString);
        }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            string email = Request.Form["Input-Email"];
            string password = Request.Form["Input-Password"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Please provide both email and password.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await _databaseHelper.GetUserByEmailAndPassword(email, password);

            if (user != null)
            {
                // Add debug lines to print user properties
                Console.WriteLine($"User Id: {user.DbUserId}");
                Console.WriteLine($"User Name: {user.Name}");
                Console.WriteLine($"User Email: {user.Email}");
                bool isArtist = await _databaseHelper.IsUserArtist(int.Parse(user.DbUserId));
                bool isListener = await _databaseHelper.IsUserListener(int.Parse(user.DbUserId));
                string userType = isArtist ? "Artist" : isListener ? "Listener" : "Unknown";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.DbUserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, userType)
                };

                // Create the ClaimsIdentity and ClaimsPrincipal objects
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authProperites = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperites);
                //await HttpContext.SignInAsync(principal, authProperites);

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid email or password";
                return Page();
            }
        }
    }
}
