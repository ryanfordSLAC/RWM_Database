using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;
using RWM_Database.Backend.Reports;
using RWM_Database.Utility;
using static RWM_Database.Backend.ItemHandler;

namespace RWM_Database.Pages.Forms.Reports
{

     /* 
    * Class description: Displays an item report page from given parameters
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class ViewReportModel : PageModel
    {

        [BindProperty(Name = "ReportType", SupportsGet = true)]
        public string ReportType { get; set; }

        //item generation start date
        [BindProperty(Name = "StartDate", SupportsGet = true)]
        public string StartDate { get; set; }


        //item generation end date
        [BindProperty(Name = "EndDate", SupportsGet = true)]
        public string EndDate { get; set; }

        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        //the list of items fitting the report criteria to display in the table
        public List<WasteDeclarationData> ItemsReport { get; set; }

        public PaginatedTable PaginatedTable { get; set; }

        public IActionResult OnGet()
        {
            if (ReportType == "0")
            {
                ItemsReport = ItemHandler.LoadItemsBetweenDates(StartDate, EndDate);
            }
            else if (ReportType == "Items Buried")
            {
               
            }
            else if (ReportType == "1")
            {
                ItemsReport = ItemHandler.LoadItemsCondition("WHERE hazardous_material = TRUE");
            }
            else if (ReportType == "2")
            {

            }
            else if (ReportType == "3")
            {
               
            }
            PaginatedTable = new PaginatedTable(10, ItemsReport.Count);
            return Page();
        }

    }
}
