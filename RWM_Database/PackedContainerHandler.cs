using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database
{
    public class PackedContainerHandler
    {

        public Dictionary<string, float> ContainerMap { get; set; }

        public List<PackedWasteFormData> PackedWasteForms { get; set; }

        public PackedContainerHandler(string shipmentNumber)
        {
            this.ContainerMap = new Dictionary<string, float>();
            this.LoadContainerList(shipmentNumber);
        }

        public class PackedWasteFormData
        {

            public string DeclarationNumber { get; set; }
            public string ContainerNumber { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }

            public PackedWasteFormData(string declaration_number, string container_number, float length, float width, float height)
            {
                this.DeclarationNumber = declaration_number;
                this.ContainerNumber = container_number;
                this.Length = length;
                this.Width = width;
                this.Height = height;
            }

            public float GetVolume()
            {
                return Length * Width * Height;
            }
        }

        public float GetUsedVolume(string container)
        {
            float totalVolume = 0;
            foreach  (PackedWasteFormData form in this.PackedWasteForms)
            {
                if (!form.ContainerNumber.Equals(container)) continue;
                totalVolume += form.GetVolume();
            }
            return totalVolume;
        }

        public float GetTotalVolume(string container)
        {
            this.ContainerMap.TryGetValue(container, out float value);
            return value;
        }

        public float GetPercentUsedVolume(string container)
        {
            float usedVolume = this.GetUsedVolume(container);
            float totalVolume = this.GetTotalVolume(container);
            return usedVolume / totalVolume * 100;
        }

        public int CountFilledContainers()
        {
            int count = 0;
            foreach(var container in ContainerMap.Keys)
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
            foreach (var container in ContainerMap.Keys)
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
            foreach (var container in ContainerMap.Keys)
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
            foreach (var container in ContainerMap.Keys)
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

        private void LoadContainerList(string shipmentNumber)
        {

            PackedWasteForms = new List<PackedWasteFormData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM container LEFT JOIN items ON container.container_number = items.container_number LEFT JOIN container_type ON container.type = container_type.type";
                if (shipmentNumber != null)
                {
                    command.CommandText += " WHERE container.shipment_number = @ShipmentNumber";
                    command.Parameters.AddWithValue("@ShipmentNumber", shipmentNumber);
                }

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        string containerNumber = read.GetString("container_number");
                        if (!read.IsDBNull(read.GetOrdinal("inner_volume")))
                        {
                            float innerVolume = read.GetFloat("inner_volume");
                            this.ContainerMap.TryAdd(containerNumber, innerVolume);
                        }

                        if (!read.IsDBNull(read.GetOrdinal("declaration_number")))
                        {
                            string declarationNumber = read.GetString("declaration_number");
                            float length = read.GetFloat("length");
                            float width = read.GetFloat("width");
                            float height = read.GetFloat("height");
                            PackedWasteFormData packedForm = new PackedWasteFormData(declarationNumber, containerNumber, length, width, height);
                            PackedWasteForms.Add(packedForm);
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

