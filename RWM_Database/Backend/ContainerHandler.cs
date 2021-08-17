using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Backend
{
    public class ContainerHandler
    {

        public List<ContainerData> ContainerList { get; set; }

        public SearchByField Search { get; set; }

        public ContainerHandler()
        {
            Search = new SearchByField();
        }

        public class ContainerData
        {
            public int ContainerId { get; set; }
            public string ContainerNumber { get; set; }
            public string SealNumber { get; set; }
            public int TypeId { get; set; }
            public string TypeName { get; set; }
            public string DatePacked { get; set; }
            public string PackedBy { get; set; }
            public string DateCreated { get; set; }
            public string UserId { get; set; }
            public int ShipmentId { get; set; }
            public string ShipmentName { get; set; }

            public ContainerData(int containerId, string containerNumber, string sealNumber, int type, string typeName, string datePacked, string packedBy, string dateCreated, string userId, int shipmentId, string shipmentName)
            {
                this.ContainerId = containerId;
                this.ContainerNumber = containerNumber;
                this.SealNumber = sealNumber;
                this.TypeId = type;
                this.TypeName = typeName;
                this.DatePacked = Util.ReformatDate(datePacked);
                this.PackedBy = packedBy;
                this.DateCreated = dateCreated;
                this.UserId = userId;
                this.ShipmentId = shipmentId;
                this.ShipmentName = shipmentName;
            }
        }


        public static Dictionary<string, int> GetAllContainersMap()
        {
            Dictionary<string, int> containerMap = new Dictionary<string, int>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT container_id, container_number FROM container";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        int containerId = read.GetInt32("container_id");
                        string containerNumber = read.GetString("container_number");
                        containerMap.TryAdd(containerNumber, containerId);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return containerMap;
        }

        public static void UpdateContainerShipment(int id, int shipmentId)
        {
           
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText =  "UPDATE container SET shipment_ref =@ShipmentId WHERE container_id=@ContainerId";
                command.Parameters.AddWithValue("@ShipmentId", shipmentId);
                command.Parameters.AddWithValue("@ContainerId", id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }


        public static void CreateContainer(IFormCollection data)
        {
            try
            {

                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO container VALUES(0, @ContainerNumber, @SealNumber, @Type, @DatePacked, @PackedBy, @DateCreated, @UserID, @ShipmentNumber)");
                command.Parameters.AddWithValue("@ContainerNumber", data["ContainerNumber"]);
                command.Parameters.AddWithValue("@SealNumber", data["SealNumber"]);
                command.Parameters.AddWithValue("@Type", data["TypeId"]);
                command.Parameters.AddWithValue("@DatePacked", data["DatePacked"]);
                command.Parameters.AddWithValue("@PackedBy", data["PackedBy"]);
                command.Parameters.AddWithValue("@DateCreated", Util.GetCurrentDate());
                command.Parameters.AddWithValue("@UserID", -1);
                command.Parameters.AddWithValue("@ShipmentNumber", -1);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        private static ContainerData CreateContainerDataObject(MySqlDataReader read)
        {
            int containerId = read.GetInt32("container_id");
            string containerNumber = read.GetString("container_number");
            string sealNumber = read.GetString("seal_number");
            int type = read.GetInt32("type_ref");
            string datePacked = read.GetString("date_packed");
            string packedBy = read.GetString("packed_by");
            string dateCreated = read.GetString("date_created");
            string userId = read.GetString("user_ref");
            int shipmentId = read.GetInt32("shipment_ref");

            string typeName = "Invalid Container Type";

            if (MySQLHandler.ColumnExists(read, "type_name"))
            {
                if (!read.IsDBNull(read.GetOrdinal("type_name")))
                {
                    typeName = read.GetString("type_name");
                }
            }

            string shipmentName = "No Shipment";

            if (MySQLHandler.ColumnExists(read, "shipment_number"))
            {
                if (!read.IsDBNull(read.GetOrdinal("shipment_number")))
                {
                    shipmentName = read.GetString("shipment_number");
                }
            }

            return new ContainerData(containerId, containerNumber, sealNumber, type, typeName, datePacked, packedBy, dateCreated, userId, shipmentId, shipmentName);
        }

        public void LoadContainers()
        {
            ContainerList = new List<ContainerData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                string baseCommand = "SELECT * FROM container LEFT JOIN container_type ON container.type_ref = container_type.container_type_id LEFT JOIN shipment ON container.shipment_ref = shipment.shipment_id";
                command.CommandText = MySQLHandler.GetSearchCommand(baseCommand, Search, command);

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        ContainerList.Add(CreateContainerDataObject(read));
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static ContainerData LoadContainerData(int containerId)
        {
            ContainerData data = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM container LEFT JOIN container_type ON container.type_ref = container_type.container_type_id WHERE container_id = @ContainerId ";
                command.Parameters.AddWithValue("@ContainerId", containerId);

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        data = CreateContainerDataObject(read);
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
