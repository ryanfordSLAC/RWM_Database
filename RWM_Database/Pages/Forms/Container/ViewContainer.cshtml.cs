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
    * Class description: Displays information about a container from a given container id
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class ViewContainerModel : PageModel
    {
        //container id, references the MySQL container_id field
        [BindProperty(Name = "ContainerId", SupportsGet = true)]
        public int ContainerId{ get; set; }

        //table pagination
        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public ContainerData ContainerData { get; set; }

        //list of items to potentially add one to the container
        public Dictionary<string, int> ItemMap;

        public Dictionary<int, string> attachmentTypes;

        //for attaching a file to the container
        public IFormFile File { get; set; }
        
        //list of already attached files
        public List<AttachmentData> AttachmentList = new List<AttachmentData>();

        //for displaying items already in the container
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

        //sets the item's referenced container to this container's id
        public IActionResult OnGetSubmitItem(int itemId, int containerId)
        {
            ItemHandler.UpdateItemContainer(itemId, containerId);
            return RedirectToPage("ViewContainer", new { ContainerId = containerId });
        }

        //sets the item's referenced container to null id
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
