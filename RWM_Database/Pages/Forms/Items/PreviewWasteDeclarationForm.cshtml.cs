using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using RWM_Database.Backend;
using RWM_Database.Backend.Attachments;
using RWM_Database.Pages.Forms;
using RWM_Database.Utility;
using static RWM_Database.Backend.Attachments.AttachmentHandler;
using static RWM_Database.Backend.ItemHandler;
using static RWM_Database.Backend.PeopleHandler;

namespace RWM_Database.Pages.Forms
{


    /* 
    * Class description: Displays an existing item declaration from a given item_id in the MySQL table
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class PreviewWasteDeclarationFormModel : PageModel
    {
        //references item_id in MySQL items table
        [BindProperty(Name = "ItemId", SupportsGet = true)]
        public int ItemId { get; set;}

        //table pagination
        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public WasteDeclarationData Form { get; set; }

        //list of attached files
        public List<AttachmentData> AttachmentList { get; set; }
        public PaginatedTable PaginatedTable { get; set; }

        public Dictionary<int, string> attachmentTypes;

        //convert from people id to LastName FirstName
        public List<PeopleData> people;

        public IFormFile File { get; set; }

        public IActionResult OnGet()
        {
            AttachmentList = new List<AttachmentData>();
            Form = ItemHandler.LoadItemWithAttachments(ItemId, AttachmentList);
            if (Form == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Item not found. item id given: " + ItemId) });
            }
            PaginatedTable = new PaginatedTable(10, AttachmentList.Count);
            attachmentTypes = ListTypeHandler.GetIdMap("attachment_type");

            people = PeopleHandler.LoadPeopleCondition(null);

            return Page();
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
                    AttachmentHandler.UploadFileToDB(File, this.ItemId, attachmentType, "SLAC Employee");
                }
            }
            return RedirectToPage("PreviewWasteDeclarationForm", new { ItemId = this.ItemId });
        }

        //convert to cubic feet 12 * 12 * 12
        public float GetVolumeConversion()
        {
            return Util.GetVolumeConversion();
        }

        public string FindPersonById(int id)
        {
            PeopleData data = people.Find(p => p.PeopleId == id);
            return data.FirstName + " " + data.LastName;
        }

    }
}