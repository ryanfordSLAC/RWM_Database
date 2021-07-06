using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using RWM_Database.Pages.Forms;
using static RWM_Database.Pages.Forms.WasteDeclarationFormModel;

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

        public void OnGet()
        {
            Form = LoadDeclarationForm(DeclarationNumber);
        }

        private WasteDeclarationData LoadDeclarationForm(string declarationNumber)
        {
            WasteDeclarationData data = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM items WHERE declaration_number = '" + declarationNumber + "'";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        string containerNumber = read.GetString(2);
                        string location = read.GetString(3);
                        string itemDescription = read.GetString(4);
                        string generationProcess = read.GetString(5);
                        data = new WasteDeclarationData(declarationNumber, containerNumber, location, itemDescription, generationProcess);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return data;
        }

    }
}