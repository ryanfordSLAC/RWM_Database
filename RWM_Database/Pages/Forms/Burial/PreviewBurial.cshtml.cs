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

        public Dictionary<string, int> ShipmentMap;


        public void OnGet()
        {
            Burial = BurialHandler.LoadBurial(BurialId);
            //check not null
            BuriedShipments = new BuriedShipmentHandler(BurialId);
            ShipmentMap = ShipmentHandler.GetAllShipmentsMap();
            PaginatedTable = new PaginatedTable(10, BuriedShipments.BuriedShipments.Count);
        }

        public IActionResult OnGetSubmitShipment(int shipmentId, int burialId)
        {
            ShipmentHandler.UpdateShipmentBurial(shipmentId, burialId);
            return RedirectToPage("PreviewBurial", new { BurialId = burialId });
        }

        public IActionResult OnGetRemoveShipment(int shipmentId, int burialId)
        {
            ShipmentHandler.UpdateShipmentBurial(shipmentId, -1);
            return RedirectToPage("PreviewBurial", new { BurialId = burialId });
        }
    }
}
