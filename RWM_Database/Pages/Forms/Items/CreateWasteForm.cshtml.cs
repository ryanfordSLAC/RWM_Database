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
    public class CreateWasteFormModel : PageModel
    {

        public List<string> ContainerList { get; set; }

        public void OnGet()
        {
            ContainerList = this.GetContainerList();
        }

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            this.CreateWasteDeclarationItem(data);
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
   * Load all containers from the table to show in the waste form creator
   */

        private List<string> GetContainerList()
        {

            List<string> containerList = new List<string>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT container_number from container";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        string containerNumber = read.GetString(0);

                        containerList.Add(containerNumber);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return containerList;
        }
    }

}
