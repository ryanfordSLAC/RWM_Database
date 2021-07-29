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
    public class CreateShipmentModel : PageModel
    {
        public void OnGet()
        {

        }

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            ShipmentHandler.CreateShipmentDBEntry(data);
            return RedirectToPage("PreviewShipment", new { ShipmentNumber = data["ShipmentNumber"] });
        }
    }
}
