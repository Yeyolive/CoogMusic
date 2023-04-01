using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
                // Add the login logic here, e.g., sign in the user
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
