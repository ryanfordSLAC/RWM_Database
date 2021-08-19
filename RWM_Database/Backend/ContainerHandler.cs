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
            public int SealNumberId { get; set; }

            public string SealNumber { get; set; }
            public int TypeId { get; set; }
            public string TypeName { get; set; }
            public string DatePacked { get; set; }
            public string PackedBy { get; set; }
            public string PackedByName { get; set; }
            public string DateCreated { get; set; }
            public string UserId { get; set; }
            public int ShipmentId { get; set; }
            public string ShipmentName { get; set; }

            public ContainerData(int containerId, string containerNumber, int sealNumberId, string sealNumber, int type, string typeName, string datePacked, string packedBy, string packedByName, string dateCreated, string userId, int shipmentId, string shipmentName)
            {
                this.ContainerId = containerId;
                this.ContainerNumber = containerNumber;
                this.SealNumberId = sealNumberId;
                this.SealNumber = sealNumber;
                this.TypeId = type;
                this.TypeName = typeName;
                this.DatePacked = Util.ReformatDate(datePacked);
                this.PackedBy = packedBy;
                this.PackedByName = packedByName;
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
            int sealNumberId = read.GetInt32("seal_number_id");
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

            string sealNumber = "Invalid Seal Number";

            if (MySQLHandler.ColumnExists(read, "seal_number"))
            {
                if (!read.IsDBNull(read.GetOrdinal("seal_number")))
                {
                    sealNumber = read.GetString("seal_number");
                }
            }

            string packedByName = "Invalid Packed By";

            if (MySQLHandler.ColumnExists(read, "first_name") && MySQLHandler.ColumnExists(read, "last_name"))
            {
                if (!read.IsDBNull(read.GetOrdinal("first_name")) && !read.IsDBNull(read.GetOrdinal("last_name")))
                {
                    packedByName = read.GetString("first_name") + " " + read.GetString("last_name");
                }
            }

            return new ContainerData(containerId, containerNumber, sealNumberId, sealNumber, type, typeName, datePacked, packedBy, packedByName, dateCreated, userId, shipmentId, shipmentName);
        }

        public void LoadContainers()
        {
            ContainerList = new List<ContainerData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                string baseCommand = "SELECT * FROM container LEFT JOIN container_type ON container.type_ref = container_type.container_type_id LEFT JOIN shipment ON container.shipment_ref = shipment.shipment_id LEFT JOIN seal_number_type ON container.seal_number_id = seal_number_type.seal_number_id LEFT JOIN people ON container.packed_by = people.people_id";
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

        public static ContainerData LoadContainerData(int containerId, List<AttachmentData> attachments)
        {
            ContainerData data = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM container LEFT JOIN attachments ON container.container_id = attachments.item_reference LEFT JOIN attachment_type ON attachments.type = attachment_type.attachment_type_id LEFT JOIN container_type ON container.type_ref = container_type.container_type_id LEFT JOIN seal_number_type ON container.seal_number_id = seal_number_type.seal_number_id LEFT JOIN people ON container.packed_by = people.people_id WHERE container_id = @ContainerId ";
                command.Parameters.AddWithValue("@ContainerId", containerId);

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        data = CreateContainerDataObject(read);

                        AttachmentData attachmentData = AttachmentHandler.CreateAttachmentObject(read);
                        if (attachmentData != null)
                        {
                            if (attachmentData.GetAttachmentTypeName() == Settings.GetStringSetting("Container_Attachment_Type"))
                            {
                                attachments.Add(attachmentData);
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

            return data;
        }


    }
}
