using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace CoogMusic.Pages.Account
{
	public class LoginTwoModel : PageModel
    {
        private readonly DbHelper _databaseHelper;

        public LoginTwoModel(IConfiguration configuration)
        {
            _databaseHelper = new DbHelper();
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
                bool isArtist = await _databaseHelper.IsUserArtist(int.Parse(user.DbUserId));
                bool isListener = await _databaseHelper.IsUserListener(int.Parse(user.DbUserId));
                string userType = isArtist ? "Artist" : isListener ? "Listener" : "Unknown";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserType", userType)
                    // Add more claims if needed, e.g., roles or other user-specific data
                };

                // Create the ClaimsIdentity and ClaimsPrincipal objects
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(principal, new AuthenticationProperties { IsPersistent = true });

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
