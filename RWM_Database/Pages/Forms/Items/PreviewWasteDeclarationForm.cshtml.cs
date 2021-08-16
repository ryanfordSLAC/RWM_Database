﻿using System;
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

namespace RWM_Database.Pages.Forms
{


    /* 
    * Displays a preview of the Waste declaration form
    * from the given declaration number directed 
    * from a different webpage
    * Author: James Meadows
    */

    public class PreviewWasteDeclarationFormModel : PageModel
    {

        [BindProperty(Name = "ItemId", SupportsGet = true)]
        public int ItemId { get; set;}

        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public WasteDeclarationData Form { get; set; }
        public List<AttachmentData> AttachmentList { get; set; }
        public PaginatedTable PaginatedTable { get; set; }

        public IFormFile File { get; set; }

        public IActionResult OnGet()
        {
            AttachmentList = new List<AttachmentData>();
            Form = ItemHandler.LoadItemWithAttachments(ItemId, AttachmentList);
            if (Form == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Item not found. item id given: " + ItemId) });
            }

            PaginatedTable = new PaginatedTable(2, AttachmentList.Count);

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
                else AttachmentHandler.UploadFileToDB(File, this.ItemId, "James Meadows");
            }
            return RedirectToPage("PreviewWasteDeclarationForm", new { ItemId = this.ItemId });
        }



    }
}