using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RWM_Database
{

    /* 
    * Class description: Provides useful functions called throughout the program for MySQL management
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class MySQLHandler
    {

        private static string connectionString;


        public static MySqlConnection GetMySQLConnection()
        {

            string login = GetLoginCredentials();
            MySqlConnection connection = new MySqlConnection(login);

            connection.Open();

            return connection;
        }


        /* 
        * load the connection string from the appsettings file
        * if it has not been loaded already
        */

        public static string GetLoginCredentials()
        {
            if (connectionString == null)
            {
                var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                connectionString = builder.Build().GetConnectionString("Database");
            }

            return connectionString;
        }

        /* 
        * Used in the search bars of the dashboard views. Adds a search parameter to the MySQL query
        */

        public static string GetSearchCommand(string start, SearchByField search, MySqlCommand command)
        {
            string sql = start;

            Dictionary<string, string> searchMap = search.GetSearchMap();

            if (searchMap.Count > 0)
            {
                sql += " WHERE ";
            }

            int count = 0;
            foreach (string field in searchMap.Keys)
            {
                if (count < searchMap.Count)
                {

                }
                bool hasVal = searchMap.TryGetValue(field, out string value);

                if (hasVal)
                {
                    sql += field + "=@" + field;
                    command.Parameters.AddWithValue("@" + field, value);
                }
                count++;

                if (count < searchMap.Count)
                {
                    sql += " AND "; 
                }
            }
            return sql;
        }

        public static bool ColumnExists(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName))
                {
                    return true;
                }
            }

            return false;
        }

        /* 
        * Cleaner way to read from the database. Not used in every mysql command
        */

        public static void ReadFromDatabase(Action<MySqlCommand> onCreate, Action<MySqlDataReader> onRead)
        {
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                onCreate(command);


                MySqlDataReader read = command.ExecuteReader();
                
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        onRead(read);
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
