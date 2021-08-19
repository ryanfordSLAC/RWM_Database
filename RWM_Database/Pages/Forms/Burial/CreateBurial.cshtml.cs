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
    public class CreateBurialModel : PageModel
    {

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
