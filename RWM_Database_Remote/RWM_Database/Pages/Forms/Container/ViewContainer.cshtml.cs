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
    * Displays information about a container 
    * to the user from a given container number
    * Author: James Meadows
    */

    public class ViewContainerModel : PageModel
    {
        [BindProperty(Name = "ContainerNumber", SupportsGet = true)]
        public string ContainerNumber { get; set; }

        public ContainerData Form { get; set; }

        public class ContainerData
        {

            public string ContainerNumber { get; set; }
            public string SealNumber { get; set; }
            public string Type { get; set; }

            public ContainerData(string containerNumber, string sealNumber, string type)
            {
                this.ContainerNumber = containerNumber;
                this.SealNumber = sealNumber;
                this.Type = type;
            }
        }

        public void OnGet()
        {
            Form = LoadContainerData(ContainerNumber);
        }

        private ContainerData LoadContainerData(string containerNumber)
        {
            ContainerData data = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM container WHERE container_number = @ContainerNumber";
                command.Parameters.AddWithValue("@ContainerNumber", containerNumber);

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        string sealNumber = read.GetString(2);
                        string type = read.GetString(3);
                        data = new ContainerData(containerNumber, sealNumber, type);
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
