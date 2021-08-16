using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RWM_Database.Backend.Attachments;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RWM_Database.Backend.Attachments.AttachmentHandler;

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
            public int ItemId { get; set; }
            public string DeclarationNumber { get; set; }
            public int ContainerId { get; set; }
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


            public WasteDeclarationData(int itemId, string declarationNumber, int containerId, string containerNumber, string location, string itemDescription, string accountNumber)
            {
                this.ItemId = itemId;
                this.DeclarationNumber = declarationNumber;
                this.ContainerId = containerId;
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

        public static void UpdateItemContainer(int itemId, int containerId)
        {

            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE items SET container_ref=@ContainerId WHERE item_id=@ItemId";
                command.Parameters.AddWithValue("@ContainerId", containerId);
                command.Parameters.AddWithValue("@ItemId", itemId);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
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
                string baseCommand = "SELECT * FROM items LEFT JOIN container ON items.container_ref = container.container_id";
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

        public static List<WasteDeclarationData> LoadItemsCondition(string condition)
        {
            string baseCommand = "SELECT * FROM items LEFT JOIN container ON items.container_ref = container.container_id";

            void onCreate(MySqlCommand command)
            {
                if (condition != null)
                {
                    baseCommand += " " + condition;
                }
                command.CommandText = baseCommand;
            }

            List<WasteDeclarationData> list = LoadItems(onCreate);

            return list;
        }

        public static List<WasteDeclarationData> LoadItemsBetweenDates(string start, string end)
        {
            string baseCommand = "SELECT * FROM items LEFT JOIN container ON items.container_ref = container.container_id";

            void onCreate(MySqlCommand command)
            {
                if (start != null && end != null)
                {
                    baseCommand += " WHERE generation_date BETWEEN @StartDate AND @EndDate";
                    command.Parameters.AddWithValue("@StartDate", start);
                    command.Parameters.AddWithValue("@EndDate", end);
                }
                command.CommandText = baseCommand;
            }

            List<WasteDeclarationData> list = LoadItems(onCreate);

            return list;
        }

        public static List<WasteDeclarationData> LoadItems(Action<MySqlCommand> onCreate)
        {
            List<WasteDeclarationData> list = new List<WasteDeclarationData>();

            void onRead(MySqlDataReader read)
            {
                WasteDeclarationData data = CreateItemDataObject(read);
                list.Add(data);
            }

            MySQLHandler.ReadFromDatabase(onCreate, onRead);

            return list;
        }

        public static WasteDeclarationData LoadItemWithAttachments(int itemId, List<AttachmentData> attachments)
        {
            WasteDeclarationData form = null;

            string baseCommand = "SELECT * FROM items LEFT JOIN attachments ON items.item_id = attachments.item_reference LEFT JOIN container ON items.container_ref = container.container_id WHERE items.item_id = @id";

            void onCreate(MySqlCommand command)
            {
                command.Parameters.AddWithValue("@id", itemId);
                command.CommandText = baseCommand;
            }

            void onRead(MySqlDataReader read)
            {
                form = CreateItemDataObject(read);
                AttachmentData attachmentData = AttachmentHandler.CreateAttachmentObject(read);
                if (attachmentData != null)
                {
                    attachments.Add(attachmentData);
                }
            }

            MySQLHandler.ReadFromDatabase(onCreate, onRead);

            return form;
        }

        public static WasteDeclarationData CreateItemDataObject(MySqlDataReader read)
        {
            int itemId = read.GetInt32("item_id");
            string declarationNumber = read.GetString("declaration_number");
            int containerId = read.GetInt32("container_ref");
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

            string containerNumber = "No Container Added";

            if (MySQLHandler.ColumnExists(read, "container_number"))
            {
                if (!read.IsDBNull(read.GetOrdinal("container_number")))
                {
                    containerNumber = read.GetString("container_number");
                }
            }


            WasteDeclarationData data = new WasteDeclarationData(itemId, declarationNumber, containerId, containerNumber, location, itemDescription, accountNumber);
            data.SetItemInformation(harzardousMaterial, generatorName, generationDate, recievedBy, recievedDate);
            data.SetDimensions(length, width, height);

            return data;
        }


        public static Dictionary<string, int> GetAllItemsMap()
        {
            Dictionary<string, int> itemsMap = new Dictionary<string, int>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT item_id, declaration_number FROM items";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        int itemId = read.GetInt32("item_id");
                        string itemNumber = read.GetString("declaration_number");
                        itemsMap.TryAdd(itemNumber, itemId);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return itemsMap;
        }

    }
}
