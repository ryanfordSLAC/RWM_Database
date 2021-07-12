using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;


/* 
 * Temporary setup to handle attachment uploading
 * Author: James Meadows
*/

namespace RWM_Database.Pages.Forms
{
    public class AttachmentModel : PageModel
    {

        public IFormFile File { get; set; }
        public List<AttachmentData> Attachments { get; set; }

        public String PreviewAttachment { get; set; }

        public class AttachmentData
        {

            readonly String name;
            readonly String date;
            readonly String creator;
            public AttachmentData(String name, String date, String creator)
            {
                this.name = name;
                this.date = date;
                this.creator = creator;
            }

            public String GetAttachmentName()
            {
                return name;
            }

            public String GetAttachmentDate()
            {
                return date;
            }

            public String GetAttachmentCreator()
            {
                return creator;
            }
        }


        public void OnGet()
        {
            Attachments = this.GetAttachmentList();
        }

        public IActionResult OnPostButton1(IFormCollection data)
        {
            return RedirectToPage("PreviewAttachment", new { FileName = data["fileName"] });
        }



        /* Action: OnPost
        * When 'File' is not null, the webserver has recieved
        * an uploaded attachment from the user. TODO: check if file duplicate name
        * TODO: set max allowed file size in appsettings
        */


        public IActionResult OnPost()
        {
            if (File != null)
            {
                //ensure file size > 0
                long length = File.Length;
                if (length < 0)
                {
                    TempData["error"] = "File size < 0";
                }
                else  UploadFileToDB(File);
            }
            return RedirectToPage("Attachment");
        }


        /*
        * Converts attachment file to byte[]
        * Then uploads to the mysql server
        */

        private void UploadFileToDB(IFormFile file)
        {
            String accountName = "James Meadows"; //TO DO add true account name when user system is created

            try
            {
                //file to byte[]
                using var fileStream = file.OpenReadStream();
                byte[] buffer = new byte[file.Length];
                fileStream.Read(buffer, 0, (int)file.Length);

                //Upload the file to the database
                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO attachments VALUES(0, @FileName, @Type, @Data, @Date, @Account_name)");
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

        /*
        * Search Database for all attachments
        * and return it as a list containing data
        * about each attachment
        */

        private List<AttachmentData> GetAttachmentList()
        {
            List<AttachmentData> attachmentList = new List<AttachmentData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM attachments";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        string fileName = read.GetString(1);
                        string attachmentDate = read.GetString(4);
                        string attachmentCreator = read.GetString(5);

                        AttachmentData data = new AttachmentData(fileName, attachmentDate, attachmentCreator);

                        attachmentList.Add(data);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return attachmentList;
        }
    }

}