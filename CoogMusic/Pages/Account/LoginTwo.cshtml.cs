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
            ApplicationUser login = new ApplicationUser();
            login.Email = Request.Form["Input-Email"];
            login.Password = Request.Form["Input-Password"];

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ApplicationUser user = await _databaseHelper.GetUserByEmailAndPassword(login.Email, login.Password);

            if (user != null)
            {
                bool isArtist = await _databaseHelper.IsUserArtist(user.Id);
                bool isListener = await _databaseHelper.IsUserListener(user.Id);
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
                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

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
