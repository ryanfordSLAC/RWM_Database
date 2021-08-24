using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;
using RWM_Database.Backend.Attachments;
using RWM_Database.Utility;
using static RWM_Database.Backend.Attachments.AttachmentHandler;
using static RWM_Database.Backend.BurialHandler;

namespace RWM_Database.Pages.Forms.Burial
{

    /* 
    * Class description: Display a burial entry from a given BurialId
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
    public class PreviewBurialModel : PageModel
    {
        //references burial_id in the MySQL table
        [BindProperty(Name = "BurialId", SupportsGet = true)]
        public int BurialId { get; set; }

        //table pagination
        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public BurialData Burial { get; set; }

        //list of shipments attached to the burial
        public BuriedShipmentHandler BuriedShipments { get; set; }

        public PaginatedTable PaginatedTable { get; set; }

        public Dictionary<string, int> ShipmentMap;


        public Dictionary<int, string> attachmentTypes;

        public IFormFile File { get; set; }

        public List<AttachmentData> AttachmentList = new List<AttachmentData>();


        public IActionResult OnGet()
        {
            Burial = BurialHandler.LoadBurial(BurialId, AttachmentList);

            if (Burial == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Burial not found. burial id given: " + BurialId) });
            }

            //check not null
            BuriedShipments = new BuriedShipmentHandler(BurialId);

            if (BuriedShipments == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Burial shipments not found. burial id given: " + BurialId) });
            }

            attachmentTypes = ListTypeHandler.GetIdMap("attachment_type");

            ShipmentMap = ShipmentHandler.GetAllShipmentsMap();
            PaginatedTable = new PaginatedTable(10, BuriedShipments.BuriedShipments.Count);

            return Page();
        }


        //updates a given shipment's referenced burial to this burial 
        public IActionResult OnGetSubmitShipment(int shipmentId, int burialId)
        {
            ShipmentHandler.UpdateShipmentBurial(shipmentId, burialId);
            return RedirectToPage("PreviewBurial", new { BurialId = burialId });
        }

        //updates a given shipment's referenced burial to null shipment
        public IActionResult OnGetRemoveShipment(int shipmentId, int burialId)
        {
            ShipmentHandler.UpdateShipmentBurial(shipmentId, -1);
            return RedirectToPage("PreviewBurial", new { BurialId = burialId });
        }

        public IActionResult OnPost(IFormCollection data)
        {
            if (File != null)
            {
                //ensure file size > 0
                long length = File.Length;
                if (length < 0)
                {
                    return RedirectToPage("/Error", new { CustomError = ("File size < 0") });
                }
                else if (length > 16000000)
                {
                    return RedirectToPage("/Error", new { CustomError = ("File Too Big. File size > 16 MB") });
                }
                else
                {
                    int attachmentType = Convert.ToInt32(data["AttachmentType"]);
                    AttachmentHandler.UploadFileToDB(File, this.BurialId, attachmentType, "SLAC Employee");
                }
            }
            return RedirectToPage("PreviewBurial", new { BurialId = this.BurialId });
        }
    }
}
