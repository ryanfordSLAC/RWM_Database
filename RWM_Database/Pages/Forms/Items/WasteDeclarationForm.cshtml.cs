using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using RWM_Database.Backend;
using RWM_Database.Utility;
using static RWM_Database.Utility.SearchByField;

namespace RWM_Database.Pages.Forms
{

    /* 
    * Class description: Dashboard for Item Declarations
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class WasteDeclarationFormModel : PageModel
    {

        public ItemHandler ItemHandler { get; set; }

        public PaginatedTable PaginatedTable { get; set; }

        //Search dashboard table by item declaration number
        [BindProperty(Name = "SearchDeclaration", SupportsGet = true)]
        public string SearchDeclaration { get; set; }

        //Search dashboard table by item referenced container
        [BindProperty(Name = "SearchContainer", SupportsGet = true)]
        public string SearchContainer { get; set; }

        //table pagination
        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }


        public void OnGet()
        {


            ItemHandler = new ItemHandler();
            SearchByField search = ItemHandler.Search;

            search.AddSearch("declaration_number", SearchDeclaration);
            search.AddSearch("container_number", SearchContainer);

            ItemHandler.LoadItemList();

            PaginatedTable = new PaginatedTable(10, ItemHandler.ItemList.Count);

        }

        public IActionResult OnPostSearchButton(IFormCollection data) {
            return RedirectToPage("WasteDeclarationForm", new { SearchDeclaration = data["SearchDeclaration"], SearchContainer = data["SearchContainer"] });
        }
         /*
        * Clears search fields
        */

        public IActionResult OnPostClearButton(IFormCollection data)
        {
            return RedirectToPage("WasteDeclarationForm");
        }

        /*
        When the View Button is pressed: redirect to a page displaying the data from a Waste form Declaration
        */

        public IActionResult OnPostViewButton(IFormCollection data)
        {
            return RedirectToPage("PreviewWasteDeclarationForm", new { DeclarationNumber = data["DeclarationNumber"] });
        }

        public IActionResult OnPostCreateButton(IFormCollection data)
        {
            
            return RedirectToPage("CreateWasteForm");
        }

    }
}