using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;
using RWM_Database.Utility;

namespace RWM_Database.Pages.Forms.Burial
{
    public class BurialDashboardModel : PageModel
    {
        public BurialHandler BurialHandler { get; set; }

        [BindProperty(Name = "SearchBurialNumber", SupportsGet = true)]
        public string SearchBurialNumber { get; set; }


        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public PaginatedTable PaginatedTable { get; set; }

        public void OnGet()
        {

            BurialHandler = new BurialHandler();
            SearchByField search = BurialHandler.Search;

            search.AddSearch("burial_number", SearchBurialNumber);

            BurialHandler.LoadBurialList();

            PaginatedTable = new PaginatedTable(2, BurialHandler.BurialList.Count);


        }

        public IActionResult OnPostSearchButton(IFormCollection data)
        {

            return RedirectToPage("BurialDashboard", new { SearchBurialNumber = data["SearchBurialNumber"]});
        }

        public IActionResult OnPostClearButton(IFormCollection data)
        {
            return RedirectToPage("BurialDashboard");
        }

        public IActionResult OnPostCreateButton()
        {

            return RedirectToPage("CreateBurial");
        }

    }
}
