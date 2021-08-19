using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MySql.Data.MySqlClient;
using RWM_Database.Backend;
using RWM_Database.Backend.Attachments;
using RWM_Database.Utility;
using static RWM_Database.Backend.Attachments.AttachmentHandler;
using static RWM_Database.Backend.ContainerHandler;

namespace RWM_Database.Pages.Forms
{

    /* 
    * Displays information about a container 
    * to the user from a given container id
    * Author: James Meadows
    */

    public class ViewContainerModel : PageModel
    {
        [BindProperty(Name = "ContainerId", SupportsGet = true)]
        public int ContainerId{ get; set; }

        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public ContainerData ContainerData { get; set; }

        public Dictionary<string, int> ItemMap;

        public Dictionary<int, string> attachmentTypes;

        public IFormFile File { get; set; }

        public List<AttachmentData> AttachmentList = new List<AttachmentData>();

        public PackedContainerHandler PackedContainerHandler { get; set; }

        public PaginatedTable PaginatedTable { get; set; }

        public IActionResult OnGet()
        {
            ContainerData = ContainerHandler.LoadContainerData(ContainerId, AttachmentList);
            if (ContainerData == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Container not found. container id given: " + ContainerId)});
            }
            PackedContainerHandler = new PackedContainerHandler(-1, ContainerId);

            if (PackedContainerHandler == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Container Items not found. container id given: " + ContainerId) });
            }

            ItemMap = ItemHandler.GetAllItemsMap();
            attachmentTypes = ListTypeHandler.GetIdMap("attachment_type");

            PaginatedTable = new PaginatedTable(10, PackedContainerHandler.PackedWasteForms.Count);

            return Page();
        }

        public IActionResult OnGetSubmitItem(int itemId, int containerId)
        {
            ItemHandler.UpdateItemContainer(itemId, containerId);
            return RedirectToPage("ViewContainer", new { ContainerId = containerId });
        }

        public IActionResult OnGetRemoveItem(int itemId, int containerId)
        {
            ItemHandler.UpdateItemContainer(itemId, -1);
            return RedirectToPage("ViewContainer", new { ContainerId = containerId });
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
                    AttachmentHandler.UploadFileToDB(File, this.ContainerId, attachmentType, "SLAC Employee");
                }
            }
            return RedirectToPage("ViewContainer", new { ContainerId = this.ContainerId });
        }
    }

}
