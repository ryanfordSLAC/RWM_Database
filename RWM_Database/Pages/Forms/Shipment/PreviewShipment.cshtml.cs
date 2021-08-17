using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;
using RWM_Database.Utility;
using static RWM_Database.Backend.ShipmentHandler;

namespace RWM_Database.Pages.Forms.Shipment
{
    public class PreviewShipmentModel : PageModel
    {

        [BindProperty(Name = "ShipmentId", SupportsGet = true)]
        public int ShipmentId { get; set; }

        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public ShipmentData Shipment { get; set; }

        public PackedContainerHandler ShipmentContainers { get; set; }

        public PaginatedTable PaginatedTable { get; set; }

        public Dictionary<string, int> ContainersMap;


        public IActionResult OnGet()
        {
            Shipment = ShipmentHandler.LoadShipment(ShipmentId);
            if (Shipment == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Shipment not found. shipment id given: " + ShipmentId) });
            }
            ShipmentContainers = new PackedContainerHandler(ShipmentId, -1);

            if (ShipmentContainers == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Shipment containers not found. shipment id given: " + ShipmentId) });
            }

            ContainersMap = ContainerHandler.GetAllContainersMap();
            PaginatedTable = new PaginatedTable(10, ShipmentContainers.PackedContainers.Count);

            return Page();
        }

        public IActionResult OnGetSubmitContainer(int containerId, int shipmentId) 
        {
            ContainerHandler.UpdateContainerShipment(containerId, shipmentId);
            return RedirectToPage("PreviewShipment", new { ShipmentId = shipmentId });
        }

        public IActionResult OnGetRemoveContainer(int containerId, int shipmentId)
        {
            ContainerHandler.UpdateContainerShipment(containerId, -1);
            return RedirectToPage("PreviewShipment", new { ShipmentId = shipmentId });
        }
    }
}
