using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using RWM_Database.Backend;
using RWM_Database.Pages.Forms;
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

        [BindProperty(Name = "DeclarationNumber", SupportsGet = true)]
        public string DeclarationNumber { get; set;}

        public WasteDeclarationData Form { get; set; }
        public List<AttachmentData> AttachmentList { get; set; } 

        public IFormFile File { get; set; }

        public void OnGet()
        {
            LoadDeclarationForm(DeclarationNumber);
        }

        public class AttachmentData
        {

            readonly string name;
            readonly byte[] data;
            readonly string date;
            readonly string creator;

            public AttachmentData(string name, byte[] data, string date, string creator)
            {
                this.name = name;
                this.data = data;
                this.date = date;
                this.creator = creator;
            }

            public string GetAttachmentName()
            {
                return name;
            }

            public byte[] GetData()
            {
                return this.data;
            }

            public string GetAttachmentDate()
            {
                return date;
            }

            public string GetAttachmentCreator()
            {
                return creator;
            }
        }

        public IActionResult OnPost(IFormCollection data)
        {
            Console.WriteLine("post");
            if (File != null)
            {
                //ensure file size > 0
                long length = File.Length;
                if (length < 0)
                {
                    TempData["error"] = "File size < 0";
                }
                else this.UploadFileToDB(File, this.DeclarationNumber);
            }
            return RedirectToPage("PreviewWasteDeclarationForm", new { DeclarationNumber = this.DeclarationNumber });
        }

        /*
        * Converts attachment file to byte[]
        * Then uploads to the mysql server
        */

        private void UploadFileToDB(IFormFile file, string itemReference)
        {
            string accountName = "James Meadows"; //TO DO add true account name when user system is created

            try
            {
                //file to byte[]
                using var fileStream = file.OpenReadStream();
                byte[] buffer = new byte[file.Length];
                fileStream.Read(buffer, 0, (int)file.Length);

                //Upload the file to the database
                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO attachments VALUES(0, @ItemReference, @FileName, @Type, @Data, @Date, @Account_name)");
                command.Parameters.AddWithValue("@ItemReference", itemReference);
                command.Parameters.AddWithValue("@FileName", file.FileName);
                command.Parameters.AddWithValue("@Type", "type");
                command.Parameters.AddWithValue("@Data", buffer);
                command.Parameters.AddWithValue("@Date", DateTime.Now.ToString("MM-dd-yyyy"));
                command.Parameters.AddWithValue("@Account_name", accountName);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private void LoadDeclarationForm(string declarationNumber)
        {
            AttachmentList = new List<AttachmentData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                //command.CommandText = "SELECT * FROM items WHERE declaration_number = '" + declarationNumber + "'";

                command.CommandText = "SELECT * FROM items LEFT JOIN attachments ON items.declaration_number = attachments.item_reference WHERE items.declaration_number = @DeclarationNumber";
                command.Parameters.AddWithValue("@DeclarationNumber", declarationNumber);
                MySqlDataReader read = command.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        Form = ItemHandler.CreateItemDataObject(read);

                        if (!read.IsDBNull(read.GetOrdinal("file_name")))
                        {
                            
                            string fileName = read.GetString("file_name");
                            string type = read.GetString("type");
                            byte[] byte_data = (byte[])read["data"];
                            string date = read.GetString("date");
                            string account_name = read.GetString("account_name");

                            AttachmentList.Add(new AttachmentData(fileName, byte_data, date, account_name));
                        }
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }

    }
}