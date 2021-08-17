using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

    }
}
