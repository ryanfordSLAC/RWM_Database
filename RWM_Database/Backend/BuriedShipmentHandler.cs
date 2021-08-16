using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RWM_Database.Backend.ShipmentHandler;

namespace RWM_Database.Backend
{
    public class BuriedShipmentHandler
    {

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
                command.CommandText = "SELECT * FROM burial LEFT JOIN shipment ON shipment.burial_ref = burial.burial_id";
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
