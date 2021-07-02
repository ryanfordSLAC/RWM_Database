using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace RWM_Database.Pages.Forms
{


    /* 
    * Loads an image preview of an attachment
    * specified from the directing webpage
    * Author: James Meadows
    */

    public class PreviewAttachmentModel : PageModel
    {

        [BindProperty(Name = "FileName", SupportsGet = true)]
        public String FileName { get; set; }

        public byte[] ImagePreview { get; set; }

        public void OnGet()
        {
            LoadAttachment(FileName);
        }

        private void LoadAttachment(String name)
        {
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM attachments WHERE file_name = '" + name + "'";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        ImagePreview = (byte[])read["data"];
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