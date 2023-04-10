using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Data;
using System.Security.Claims;

namespace CoogMusic.Pages.Search
{
    public class AddToPlaylistModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public AddToPlaylistModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
    }
}
