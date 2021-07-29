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

        [BindProperty(Name = "ShipmentNumber", SupportsGet = true)]
        public string ShipmentNumber { get; set; }

        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public ShipmentData Shipment { get; set; }

        public PackedContainerHandler ShipmentContainers { get; set; }

        public PaginatedTable PaginatedTable { get; set; }


        public void OnGet()
        {
            Console.WriteLine(ShipmentNumber);
            Shipment = ShipmentHandler.LoadShipment(ShipmentNumber);
            ShipmentContainers = new PackedContainerHandler(ShipmentNumber);
            PaginatedTable = new PaginatedTable(10, ShipmentContainers.ContainerMap.Count);
        }
    }
}
