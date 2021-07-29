using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;
using RWM_Database.Utility;
using static RWM_Database.Backend.BurialHandler;

namespace RWM_Database.Pages.Forms.Burial
{
    public class PreviewBurialModel : PageModel
    {

        [BindProperty(Name = "BurialId", SupportsGet = true)]
        public int BurialId { get; set; }

        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public BurialData Burial { get; set; }

        public BuriedShipmentHandler BuriedShipments { get; set; }

        public PaginatedTable PaginatedTable { get; set; }


        public void OnGet()
        {
            Burial = BurialHandler.LoadBurial(BurialId);
            BuriedShipments = new BuriedShipmentHandler(BurialId);
            PaginatedTable = new PaginatedTable(10, BuriedShipments.BuriedShipments.Count);
        }
    }
}
