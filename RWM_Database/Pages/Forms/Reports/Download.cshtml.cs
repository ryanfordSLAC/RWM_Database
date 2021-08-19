using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RWM_Database.Pages.Forms.Reports
{
    public class DownloadModel : PageModel
    {
        [BindProperty(Name = "FilePath", SupportsGet = true)]
        public string FilePath { get; set; }
        public void OnGet()
        {

        }
    }
}
