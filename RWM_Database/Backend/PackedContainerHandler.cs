using MySql.Data.MySqlClient;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database
{
    public class PackedContainerHandler
    {

        

        public List<PackedWasteFormData> PackedWasteForms { get; set; }

        public Dictionary<int, PackedContainer> PackedContainers { get; set; }

        public PackedContainerHandler(int shipmentNumber, int containerId)
        {
            this.PackedContainers = new Dictionary<int, PackedContainer>();
            this.LoadContainerList(shipmentNumber, containerId);
        }

        public class PackedContainer
        {

            public int ContainerId { get; set; }
            public string ContainerNumber { get; set; }
            public float ContainerVolume { get; set; }

            public PackedContainer(int containerId, string containerNumber, float containerVolume)
            {
                this.ContainerId = containerId;
                this.ContainerNumber = containerNumber;
                this.ContainerVolume = containerVolume;
            }
        }


        public class PackedWasteFormData
        {

            public int ItemId { get; set; }
            public string DeclarationNumber { get; set; }
            public int ContainerId{ get; set; }
            public string ContainerNumber { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }

            public PackedWasteFormData(int itemId, string declaration_number, int containerId, string container_number, float length, float width, float height)
            {
                this.ItemId = itemId;
                this.DeclarationNumber = declaration_number;
                this.ContainerId = containerId;
                this.ContainerNumber = container_number;
                this.Length = length;
                this.Width = width;
                this.Height = height;
            }

            public float GetVolume()
            {
                return (Length * Width * Height) / Util.GetVolumeConversion();
            }
        }

        public float GetUsedVolumeAllContainers()
        {
            float usedVolume = 0f;
            foreach (int container in this.PackedContainers.Keys)
            {
                usedVolume += GetUsedVolume(container);
            }
            return usedVolume;
        }

        public float GetUsedVolume(int container)
        {
            float totalVolume = 0;
            foreach  (PackedWasteFormData form in this.PackedWasteForms)
            {
                if (!form.ContainerId.Equals(container)) continue;
                totalVolume += form.GetVolume();
            }
            return totalVolume;
        }

        public float GetTotalVolume(int container)
        {
            this.PackedContainers.TryGetValue(container, out PackedContainer value);
            return value.ContainerVolume;
        }

        public float GetPercentUsedVolume(int container)
        {
            float usedVolume = this.GetUsedVolume(container);
            float totalVolume = this.GetTotalVolume(container);
            return (usedVolume / totalVolume) * 100;
        }

        public int CountFilledContainers()
        {
            int count = 0;
            foreach(int container in PackedContainers.Keys)
            {
                float totalVolume = this.GetTotalVolume(container);
                float usedVolume = this.GetUsedVolume(container);
                if (totalVolume == usedVolume)
                {
                    count++;
                }
            }
            return count;
        }

        public int CountEmptyContainers()
        {
            int count = 0;
            foreach (int container in PackedContainers.Keys)
            {
                float usedVolume = this.GetUsedVolume(container);
                if (usedVolume == 0)
                {
                    count++;
                }
            }
            return count;
        }

        public int CountOvercapacityContainers()
        {
            int count = 0;
            foreach (int container in PackedContainers.Keys)
            {
                float totalVolume = this.GetTotalVolume(container);
                float usedVolume = this.GetUsedVolume(container);
                if (totalVolume < usedVolume)
                {
                    count++;
                }
            }
            return count;
        }

        public int CountInProgessContainers()
        {
            int count = 0;
            foreach (int container in PackedContainers.Keys)
            {
                float totalVolume = this.GetTotalVolume(container);
                float usedVolume = this.GetUsedVolume(container);
                if (totalVolume > usedVolume && usedVolume > 0)
                {
                    count++;
                }
            }
            return count;
        }

        private void LoadContainerList(int shipmentId, int optionalContainerId)
        {

            PackedWasteForms = new List<PackedWasteFormData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM container LEFT JOIN items ON container.container_id = items.container_ref LEFT JOIN container_type ON container.type_ref = container_type.container_type_id";
                if (shipmentId != -1)
                {
                    command.CommandText += " WHERE container.shipment_ref = @ShipmentId";
                    command.Parameters.AddWithValue("@ShipmentId", shipmentId);
                }

                if (optionalContainerId != -1)
                {
                    command.CommandText += " WHERE container.container_id = @ContainerId";
                    command.Parameters.AddWithValue("@ContainerId", optionalContainerId);
                }

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        int containerId = read.GetInt32("container_id");
                        string containerNumber = read.GetString("container_number");

                        if (MySQLHandler.ColumnExists(read, "inner_volume"))
                        {
                            if (!read.IsDBNull(read.GetOrdinal("inner_volume")))
                            {
                                float innerVolume = read.GetFloat("inner_volume");
                                this.PackedContainers.TryAdd(containerId, new PackedContainer(containerId, containerNumber, innerVolume));
                            }
                        }

                        if (MySQLHandler.ColumnExists(read, "item_id"))
                        {
                            if (!read.IsDBNull(read.GetOrdinal("item_id")))
                            {
                                int itemId = read.GetInt32("item_id");
                                string declarationNumber = read.GetString("declaration_number");
                                float length = read.GetFloat("length");
                                float width = read.GetFloat("width");
                                float height = read.GetFloat("height");
                                PackedWasteFormData packedForm = new PackedWasteFormData(itemId, declarationNumber, containerId, containerNumber, length, width, height);
                                PackedWasteForms.Add(packedForm);
                            }
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

