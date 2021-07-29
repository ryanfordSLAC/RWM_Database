using MySql.Data.MySqlClient;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Backend
{
    public class ItemHandler
    {
        public List<WasteDeclarationData> ItemList { get; set; }

        public SearchByField Search { get; set; }


        public ItemHandler()
        {
            Search = new SearchByField();
        }

        public class WasteDeclarationData
        {

            public string DeclarationNumber { get; set; }
            public string ContainerNumber { get; set; }
            public string Location { get; set; }
            public string ItemDescription { get; set; }
            public string AccountNumber { get; set; }
            public bool HarzardousMaterial { get; set; }
            public string GeneratorName { get; set; }
            public string GenerationDate { get; set; }
            public string RecievedBy { get; set; }
            public string RecievedDate { get; set; }

            public float Length { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }


            public WasteDeclarationData(string declarationNumber, string containerNumber, string location, string itemDescription, string accountNumber)
            {
                this.DeclarationNumber = declarationNumber;
                this.ContainerNumber = containerNumber;
                this.Location = location;
                this.ItemDescription = itemDescription;
                this.AccountNumber = accountNumber;
            }

            public void SetItemInformation(bool harzardousMaterial, string generatorName, string generationDate, string recievedBy, string recievedDate)
            {
                this.HarzardousMaterial = harzardousMaterial;
                this.GeneratorName = generatorName;
                this.GenerationDate = generationDate;
                this.RecievedBy = recievedBy;
                this.RecievedDate = recievedDate;
            }

            public void SetDimensions(float length, float width, float height)
            {
                this.Length = length;
                this.Width = width;
                this.Height = height;
            }

        }

        /*
        * Load all Waste Declaration Forms in the database and return them as a List for the website to preview
        */

        public void LoadItemList()
        {

            ItemList = new List<WasteDeclarationData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                string baseCommand = "SELECT * FROM items";
                command.CommandText = MySQLHandler.GetSearchCommand(baseCommand, Search, command);

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        WasteDeclarationData data = CreateItemDataObject(read);
                        ItemList.Add(data);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static WasteDeclarationData CreateItemDataObject(MySqlDataReader read)
        {
            string declarationNumber = read.GetString("declaration_number");
            string containerNumber = read.GetString("container_number");
            string itemDescription = read.GetString("item_description");
            string location = read.GetString("location");
            string accountNumber = read.GetString("account_number");
            bool harzardousMaterial = read.GetBoolean("hazardous_material");
            string generatorName = read.GetString("generator_name");
            string generationDate = read.GetString("generation_date");
            string recievedBy = read.GetString("recieved_by");
            string recievedDate = read.GetString("recieved_date");
            float length = read.GetFloat("length");
            float width = read.GetFloat("width");
            float height = read.GetFloat("height");


            WasteDeclarationData data = new WasteDeclarationData(declarationNumber, containerNumber, location, itemDescription, accountNumber);
            data.SetItemInformation(harzardousMaterial, generatorName, generationDate, recievedBy, recievedDate);
            data.SetDimensions(length, width, height);

            return data;
        }

    }
}
