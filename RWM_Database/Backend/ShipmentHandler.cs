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

     /* 
    * Class description: Backend MySQL for shipment table
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class ShipmentHandler
    {


        public List<ShipmentData> ShipmentList { get; set; }
        public SearchByField Search { get; set; }

        public ShipmentHandler()
        {
            Search = new SearchByField();
        }

        public class ShipmentData
        {

            public int ShipmentId { get; set; }

            public string ShipmentNumber { get; set; }
            public int ShipmentType { get; set; }
            public string ShipmentTypeName { get; set; }
            public int ShipmentMaxVolume { get; set; }

            public ShipmentData(int shipmentId, string shipmentNumber, int shipmentType,  string shipmentTypeName, int shipmentMaxVolume)
            {
                this.ShipmentId = shipmentId;
                this.ShipmentNumber = shipmentNumber;
                this.ShipmentType = shipmentType;
                this.ShipmentTypeName = shipmentTypeName;
                this.ShipmentMaxVolume = shipmentMaxVolume;
            }
        }
        //Map of shipment_id , shipment_number
        public static Dictionary<string, int> GetAllShipmentsMap()
        {
            Dictionary<string, int> shipmentMap = new Dictionary<string, int>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT shipment_id, shipment_number FROM shipment";

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        int shipmentId = read.GetInt32("shipment_id");
                        string shipmentNumber = read.GetString("shipment_number");
                        shipmentMap.TryAdd(shipmentNumber, shipmentId);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return shipmentMap;
        }

        public static void UpdateShipmentBurial(int shipmentId, int burialId)
        {

            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE shipment SET burial_ref =@BurialId WHERE shipment_id=@ShipmentId";
                command.Parameters.AddWithValue("@ShipmentId", shipmentId);
                command.Parameters.AddWithValue("@BurialId", burialId);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public void GetShipmentList()
        {

            ShipmentList = new List<ShipmentData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                string baseCommand = "SELECT * FROM shipment LEFT JOIN shipment_type ON shipment.shipment_type_ref = shipment_type.shipment_type_id";
                command.CommandText = MySQLHandler.GetSearchCommand(baseCommand, Search, command);

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        ShipmentData data = CreateShipmentDataObject(read);
                        ShipmentList.Add(data);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static ShipmentData CreateShipmentDataObject(MySqlDataReader read)
        {
            int shipmentId = read.GetInt32("shipment_id");
            string shipmentNumber = read.GetString("shipment_number");
            int shipmentType = read.GetInt32("shipment_type_ref");

            string shipmentTypeName = "None";
            int shipmentMaxVolume = 0;

            if (MySQLHandler.ColumnExists(read, "type_name"))
            {
                if (!read.IsDBNull(read.GetOrdinal("type_name")))
                {
                    shipmentTypeName = read.GetString("type_name");
                    shipmentMaxVolume = read.GetInt32("max_volume");
                }
            }

            ShipmentData data = new ShipmentData(shipmentId, shipmentNumber, shipmentType, shipmentTypeName, shipmentMaxVolume);
            return data;
        }

        public static void CreateShipmentDBEntry(IFormCollection data)
        {
            try
            {
                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO shipment VALUES(0, @ShipmentNumber, @ShipmentType, @BurialId, @DateShipped, @DateRecieved)");
                command.Parameters.AddWithValue("@ShipmentNumber", data["ShipmentNumber"]);
                command.Parameters.AddWithValue("@ShipmentType", Convert.ToInt32(data["ShipmentTypeId"]));
                command.Parameters.AddWithValue("@BurialId", -1);
                command.Parameters.AddWithValue("@DateShipped", data["DateShipped"]);
                command.Parameters.AddWithValue("@DateRecieved", data["DateRecieved"]);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        //loads a shipment from a given shipmentId. attachments is an empty list that will populate with attachments loaded here
        public static ShipmentData LoadShipment(int shipmentId, List<AttachmentData> attachments)
        {
            ShipmentData data = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM shipment LEFT JOIN attachments ON shipment.shipment_id = attachments.item_reference LEFT JOIN attachment_type ON attachments.type = attachment_type.attachment_type_id LEFT JOIN shipment_type ON shipment.shipment_type_ref = shipment_type.shipment_type_id WHERE shipment_id = @ShipmentId";
                command.Parameters.AddWithValue("@ShipmentId", shipmentId);
                MySqlDataReader read = command.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        data = CreateShipmentDataObject(read);

                        AttachmentData attachmentData = AttachmentHandler.CreateAttachmentObject(read);
                        if (attachmentData != null)
                        {
                            if (attachmentData.GetAttachmentTypeName() == Settings.GetStringSetting("Shipment_Attachment_Type"))
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
