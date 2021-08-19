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


        public Dictionary<int, string> attachmentTypes;

        public IFormFile File { get; set; }

        public List<AttachmentData> AttachmentList = new List<AttachmentData>();


        public IActionResult OnGet()
        {
            Shipment = ShipmentHandler.LoadShipment(ShipmentId, AttachmentList);
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
            attachmentTypes = ListTypeHandler.GetIdMap("attachment_type");
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
                    AttachmentHandler.UploadFileToDB(File, this.ShipmentId, attachmentType, "SLAC Employee");
                }
            }
            return RedirectToPage("PreviewShipment", new { ShipmentId = this.ShipmentId });
        }
    }
}
