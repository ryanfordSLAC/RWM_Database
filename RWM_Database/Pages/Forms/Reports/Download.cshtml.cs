using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RWM_Database.Pages.Forms.Reports
{

    /* 
    * Class description: Simple page to download a file from a given server file path
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
    public class DownloadModel : PageModel
    {
        [BindProperty(Name = "FilePath", SupportsGet = true)]
        public string FilePath { get; set; }
        public void OnGet()
        {

        }
    }
}
