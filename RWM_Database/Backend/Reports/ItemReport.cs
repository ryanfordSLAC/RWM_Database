using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Backend.Reports
{
     /* 
    * Class description: MySQL backend for generating reports for the main dashboard
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
    public class ItemReport
    {

        public int CountTotalItems()
        {
            return TableReport.Count("items", null);
        }

        public int CountHazardousItems()
        {
            return TableReport.Count("items", " WHERE hazardous_material = TRUE");
        }

        public int CountItemsBetween(string startDate, string endDate)
        {
            return TableReport.Count("items", " WHERE generation_date BETWEEN ");
        }

        public int CountBetween(string table, string startDate, string endDate)
        {

            using (MySqlConnection connection = new MySqlConnection(MySQLHandler.GetLoginCredentials()))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM " + table + " WHERE generation_date BETWEEN @StartDate AND @EndDate";
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count;
                }
            }
        }

    }
}
