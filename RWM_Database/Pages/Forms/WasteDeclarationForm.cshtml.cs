using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace RWM_Database.Pages.Forms
{

    /* 
    * Class to add Waste Declaration Items
    * to the database
    * Author: James Meadows
    */

    public class WasteDeclarationFormModel : PageModel
    {


        public List<WasteDeclarationData> WasteForms { get; set; }

        public String PreviewForm { get; set; }

        public class WasteDeclarationData
        {

            public String DeclarationNumber { get; set; }
            public String ContainerNumber { get; set; }
            public String Location { get; set; }
            public String ItemDescription { get; set; }
            public String GenerationProcess { get; set; }

            public WasteDeclarationData(String declaration_number, String container_number, String location, String item_description, String generation_process)
            {
                this.DeclarationNumber = declaration_number;
                this.ContainerNumber = container_number;
                this.Location = location;
                this.ItemDescription = item_description;
                this.GenerationProcess = generation_process;
            }

        }

        public void OnGet()
        {
            
            WasteForms = this.GetWasteFormsList();
        }

        /*
        When the Submit Button is pressed: create an entry in the database for the desired
        * Waste Declaration Form with the input data
        */

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            this.CreateWasteDeclarationItem(data);
            return RedirectToPage("WasteDeclarationForm");
        }

        /*
        When the View Button is pressed: redirect to a page displaying the data from a Waste form Declaration
        */

        public IActionResult OnPostViewButton(IFormCollection data)
        {
            return RedirectToPage("PreviewWasteDeclarationForm", new { DeclarationNumber = data["DeclarationNumber"] });
        }

        /*
        Creates the waste declaration form entry in the MySQL Database
        */

        private void CreateWasteDeclarationItem(IFormCollection data)
        {
            try
            {

                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO items VALUES(0, @DeclarationNumber, @ContainerNumber, @Location, @ItemDescription, @GenerationProcess)");
                command.Parameters.AddWithValue("@DeclarationNumber", data["DeclarationNumber"]);
                command.Parameters.AddWithValue("@ContainerNumber", data["ContainerNumber"]);
                command.Parameters.AddWithValue("@Location", data["Location"]);
                command.Parameters.AddWithValue("@ItemDescription", data["ItemDescription"]);
                command.Parameters.AddWithValue("@GenerationProcess", data["GenerationProcess"]);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }


        /*
        * Load all Waste Declaration Forms in the database and return them as a List for the website to preview
        */

        private List<WasteDeclarationData> GetWasteFormsList()
        {
            
            List<WasteDeclarationData> attachmentList = new List<WasteDeclarationData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM items";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        string declarationNumber = read.GetString(1);
                        string containerNumber = read.GetString(2);
                        string location = read.GetString(3);
                        string itemDescription = read.GetString(4);
                        string generationProcess = read.GetString(5);

                        WasteDeclarationData data = new WasteDeclarationData(declarationNumber, containerNumber, location, itemDescription, generationProcess);

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