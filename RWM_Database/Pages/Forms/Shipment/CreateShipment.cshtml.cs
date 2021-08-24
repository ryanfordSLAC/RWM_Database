using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;

namespace RWM_Database.Pages.Forms.Shipment
{


    /* 
    * Class description: Adds a shipment to the database from user input
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class CreateShipmentModel : PageModel
    {
        //used to convert from shipment type id to shipment type name
        public Dictionary<int, string> shipmentTypeMap;

        public void OnGet()
        {
            shipmentTypeMap = ListTypeHandler.GetIdMap("shipment_type");
        }

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            ShipmentHandler.CreateShipmentDBEntry(data);
            return RedirectToPage("ShipmentDashboard");//todo: add item success page
        }
    }
}
