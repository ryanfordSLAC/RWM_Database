﻿using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RWM_Database
{

    /* 
    * Used for handling the database connection
    * Author: James Meadows
    */

    public class MySQLHandler
    {

        private static string connectionString;

        public static MySqlConnection GetMySQLConnection()
        {
            //open MySQL connection. TODO: keep a persistent mysql connection open.

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
    }
}
