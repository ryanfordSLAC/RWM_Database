using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace RWM_Database.Pages.Forms.Attachment
{

        /* 
    * Class description: Loads an attachment from the attachments table in MySQL
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
    public class ViewAttachmentModel : PageModel
    {
        //attachment_id in MySQL table
        [BindProperty(Name = "FileId", SupportsGet = true)]
        public int FileId { get; set; }

        //byte[] of the file data
        public byte[] ContentData { get; set; }

        public string FileName { get; set; }

        public ActionResult OnGet()
        {
            LoadAttachment(FileId);

            if (ContentData == null || FileName == null)
            {
                return RedirectToPage("/Error", new { CustomError = ("Attachment not found. id given: " + FileId) });
            }
            if (FileName.EndsWith(".pdf"))
            {
                return DisplayPDF();
            }
            else return Page();
        }

        private void LoadAttachment(int id)
        {
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM attachments WHERE attachment_id = '" + id + "'";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        FileName = read.GetString("file_name");
                        ContentData = (byte[])read["data"];
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public ActionResult DisplayPDF()
        {
            return new FileContentResult(ContentData, "application/pdf");
        }


    }
}
