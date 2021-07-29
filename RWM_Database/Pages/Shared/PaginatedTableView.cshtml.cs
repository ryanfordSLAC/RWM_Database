using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Utility;

namespace RWM_Database.Pages.Shared
{
    public class PaginatedTableViewModel : PageModel
    {

        public SearchByField Search { get; set; }

        public void OnGet()
        {
        }
    }
}
