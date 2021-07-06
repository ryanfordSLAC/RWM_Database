using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using static RWM_Database.Pages.Forms.ViewContainerModel;

/* 
 * This class creates containers in the database
 * from user form input
 * Author: James Meadows
*/

namespace RWM_Database.Pages.Forms
{
    public class CreateContainerModel : PageModel
    {
        public void OnGet()
        {
            ContainerList = LoadContainers();
        }

        public List<ContainerData> ContainerList { get; set; }


        /*
        When the Submit Button is pressed: create an entry in the database for the desired
        * container with the input data
        */

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            this.CreateContainer(data);
            return RedirectToPage("CreateContainer");
        }


        private void CreateContainer(IFormCollection data)
        {
            try
            {

                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO container VALUES(0, @ContainerNumber, @SealNumber, @Type)");
                command.Parameters.AddWithValue("@ContainerNumber", data["ContainerNumber"]);
                command.Parameters.AddWithValue("@SealNumber", data["SealNumber"]);
                command.Parameters.AddWithValue("@Type", data["Type"]);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private List<ContainerData> LoadContainers()
        {
            List<ContainerData> list = new List<ContainerData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM container";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        string containerNumber = read.GetString(1);
                        string sealNumber = read.GetString(2);
                        string type = read.GetString(3);
                        list.Add(new ContainerData(containerNumber, sealNumber, type));
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return list;
        }

    }
}
