using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Security.Claims;

namespace CoogMusic.Pages.Artists
{
    public class IndexModel : PageModel
    {

        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

            public void OnGet()
        {
        }
    }
}
