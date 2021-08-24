using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RWM_Database.Backend.ShipmentHandler;

namespace RWM_Database.Backend
{
    /* 
    * Class description: Backend MySQL for shipments that reference a burial
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
    public class BuriedShipmentHandler
    {
        //list of shipments with a given burial_id as their 'burial_ref'
        public List<ShipmentData> BuriedShipments { get; set; }

        public BuriedShipmentHandler(int burialId)
        {
            this.LoadBurialList(burialId);
        }

        private void LoadBurialList(int burialId)
        {

            BuriedShipments = new List<ShipmentData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM burial LEFT JOIN shipment ON shipment.burial_ref = burial.burial_id LEFT JOIN shipment_type ON shipment.shipment_type_ref = shipment_type.shipment_type_id";
                if (burialId != -1)
                {
                    command.CommandText += " WHERE burial.burial_id = @BurialId";
                    command.Parameters.AddWithValue("@BurialId", burialId);
                }

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {

                        if (MySQLHandler.ColumnExists(read, "shipment_number"))
                        {
                            if (!read.IsDBNull(read.GetOrdinal("shipment_number")))
                            {
                                ShipmentData buriedShipment = ShipmentHandler.CreateShipmentDataObject(read);
                                BuriedShipments.Add(buriedShipment);
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
