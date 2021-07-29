using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend.Reports;

namespace RWM_Database.Pages.Forms.Reports
{
    public class ViewReportModel : PageModel
    {

        [BindProperty(Name = "ReportType", SupportsGet = true)]
        public string ReportType { get; set; }

        [BindProperty(Name = "StartDate", SupportsGet = true)]
        public string StartDate { get; set; }

        [BindProperty(Name = "EndDate", SupportsGet = true)]
        public string EndDate { get; set; }

        public ItemReport ItemReport { get; set; }

        public void OnGet()
        {
            ItemReport = new ItemReport();
        }
    }
}
