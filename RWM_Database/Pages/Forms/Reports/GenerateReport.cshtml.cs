using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RWM_Database.Pages.Forms.Reports
{
    public class GenerateReportModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostViewButton(IFormCollection data)
        {
            return RedirectToPage("ViewReport", new { ReportType = data["ReportType"], StartDate = data["StartDate"], EndDate = data["EndDate"] });
        }


    }
}
