using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Backend.Reports
{
    public class TableReport
    {

        public static int Count(string table, string condition)
        {

            using (MySqlConnection connection = new MySqlConnection(MySQLHandler.GetLoginCredentials()))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM " + table + condition;
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }
        }

        public static DataTable ReadAllItems(string query)
        {
            DataTable table = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();

                MySqlCommand cmd = new MySqlCommand(query, connection);
                table = new DataTable();
                table.Load(cmd.ExecuteReader());
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            return table;
        }

    }
}
