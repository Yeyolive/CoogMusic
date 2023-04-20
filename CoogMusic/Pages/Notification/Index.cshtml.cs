using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoogMusic.Pages.Notification
{
	public class IndexModel : PageModel
    {
        public String errorMessage = "";
        public String successMessage = "";
        public SongInfo songInfo = new SongInfo();

        private readonly DbHelper _databaseHelper;
        // Access this by calling connectionStr variable in your functions (GLOBAL)
        private readonly string connectionStr;

        public IndexModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connectionStr = connectionString;
            _databaseHelper = new DbHelper(connectionString);
        }

        public void OnGet()
        {
        }
    }
}
