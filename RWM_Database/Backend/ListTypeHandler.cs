using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static RWM_Database.Utility.MappedTable;

namespace RWM_Database.Backend
{
    public class ListTypeHandler
    {

        public class ListTypeData
        {
 
            public Dictionary<string, object> valueMap = new Dictionary<string, object>();
        }

        public static List<ListTypeData> LoadListTypeValues(string tableName, MappedTable table)
        {
            List<ListTypeData> list = new List<ListTypeData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM " + tableName;

                MySqlDataReader read = command.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        ListTypeData data = new ListTypeData();
                        foreach (string column in table.map.Keys)
                        {
                            MappedData type = table.map[column];
                            if (type.IsFloat())
                            {
                                data.valueMap.TryAdd(column, read.GetFloat(column));
                            }
                            else if (type.IsInt())
                            {
                                data.valueMap.TryAdd(column, read.GetInt32(column));
                            }
                            else if (type.IsString())
                            {
                                data.valueMap.TryAdd(column, read.GetString(column));
                            }
                            else if (type.IsBoolean())
                            {
                                data.valueMap.TryAdd(column, read.GetBoolean(column));
                            }
                            else throw new Exception("Invalid type used for list type table. Only supports: Floats, Int32, String, Boolean: " + type.ColumnType.Name);
                        }
                        list.Add(data);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return list;
        }

        public static void CreateListTypeEntry(string tableName, IFormCollection data, MappedTable table) 
        {
            MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
            string values = "";
                
            foreach (string column in table.map.Keys)
            {
                values += ", @" + column;
            }

            command.CommandText = ("INSERT INTO " + tableName +  " VALUES(0" + values + ")");

            foreach (string column in table.map.Keys)
            {
                command.Parameters.AddWithValue("@" + column, data[column]);
            }
              
            command.ExecuteReader();
        }


        public static Dictionary<int,string> GetIdMap(string tableName)
        {
            Dictionary<int, string> map = new Dictionary<int, string>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM " + tableName;

                MySqlDataReader read = command.ExecuteReader();

                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        int id = read.GetInt32(0);
                        string name = read.GetString(1);
                        map.TryAdd(id, name);
                    }
                }
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return map;
        }



    }
}
