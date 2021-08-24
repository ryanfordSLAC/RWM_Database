using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;

namespace RWM_Database.Pages.Forms.Burial
{

    /* 
    * Class description: Add a burial entry to the burial MySQL table
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class CreateBurialModel : PageModel
    {
        //convert from disosal rype id to readable name
        public Dictionary<int, string> disposalTypeMap;
        public void OnGet()
        {
            disposalTypeMap = ListTypeHandler.GetIdMap("disposal_site_type");
        }

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            BurialHandler.CreateBurialDBEntry(data);
            return RedirectToPage("BurialDashboard");
        }

    }
}
