using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Backend
{
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

            public string ShipmentNumber { get; set; }
            public string ShipmentType { get; set; }
            public string ShipmentConveyance { get; set; }

            public ShipmentData(string shipmentNumber, string shipmentType, string shipmentConveyance)
            {
                this.ShipmentNumber = shipmentNumber;
                this.ShipmentType = shipmentType;
                this.ShipmentConveyance = shipmentConveyance;
            }
        }


        public void GetShipmentList()
        {

            ShipmentList = new List<ShipmentData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                string baseCommand = "SELECT * FROM shipment";
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
            string shipmentNumber = read.GetString("shipment_number");
            string shipmentType = read.GetString("shipment_type");
            string shipmentConveyance = read.GetString("shipment_conveyance");

            ShipmentData data = new ShipmentData(shipmentNumber, shipmentType, shipmentConveyance);
            return data;
        }

        public static void CreateShipmentDBEntry(IFormCollection data)
        {
            try
            {

                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO shipment VALUES(0, @ShipmentNumber, @ShipmentType, @ShipmentConveyance)");
                command.Parameters.AddWithValue("@ShipmentNumber", data["ShipmentNumber"]);
                command.Parameters.AddWithValue("@ShipmentType", data["ShipmentType"]);
                command.Parameters.AddWithValue("@ShipmentConveyance", data["ShipmentConveyance"]);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static ShipmentData LoadShipment(string shipmentNumber)
        {
            ShipmentData data = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM shipment WHERE shipment_number = @ShipmentNumber";
                command.Parameters.AddWithValue("@ShipmentNumber", shipmentNumber);
                MySqlDataReader read = command.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        data = CreateShipmentDataObject(read);
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
