using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    // Add more claims if needed, e.g., roles or other user-specific data
                };

                // Create the ClaimsIdentity and ClaimsPrincipal objects
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
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
