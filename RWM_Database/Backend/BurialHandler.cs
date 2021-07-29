using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Backend
{
    public class BurialHandler
    {

        public List<BurialData> BurialList { get; set; }
        public SearchByField Search { get; set; }

        public BurialHandler()
        {
            Search = new SearchByField();
        }

        public class BurialData
        {


            public int BurialId { get; set; }

            public string BurialNumber { get; set; }

            public string Certificate { get; set; }

            public BurialData(int burialId, string burialNumber, string certificate)
            {
                this.BurialId = burialId;
                this.BurialNumber = burialNumber;
                this.Certificate = certificate;
            }
        }


        public void LoadBurialList()
        {

            BurialList = new List<BurialData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                string baseCommand = "SELECT * FROM burial";
                command.CommandText = MySQLHandler.GetSearchCommand(baseCommand, Search, command);

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        BurialData data = CreateBurialDataObject(read);
                        BurialList.Add(data);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static BurialData CreateBurialDataObject(MySqlDataReader read)
        {
            int burialId = read.GetInt32("id");
            string burialNumber = read.GetString("burial_number");
            string certificate = read.GetString("certificate");

            BurialData data = new BurialData(burialId, burialNumber, certificate);
            return data;
        }

        public static void CreateBurialDBEntry(IFormCollection data)
        {
            try
            {

                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO burial VALUES(0, @BurialNumber, @Certificate)");
                command.Parameters.AddWithValue("@BurialNumber", data["BurialNumber"]);
                command.Parameters.AddWithValue("@Certificate", data["Certificate"]);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static BurialData LoadBurial(int burialId)
        {
            BurialData data = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM burial WHERE id = @BurialId";
                command.Parameters.AddWithValue("@BurialId", burialId);
                MySqlDataReader read = command.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        data = CreateBurialDataObject(read);
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
