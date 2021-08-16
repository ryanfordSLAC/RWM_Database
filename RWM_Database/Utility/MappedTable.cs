using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Utility
{
    public class MappedTable
    {


        public Dictionary<string, MappedData> map = new Dictionary<string, MappedData>();
        private DataTable table;
        public string TableName { get; set; }
        private bool excludeFirst = false;

        public MappedTable(string tableName, bool excludeFirst)
        {
            this.TableName = tableName;
            this.excludeFirst = excludeFirst;

            table = this.LoadTable(TableName);
            if (table == null)
            {
                throw new Exception("Table was null: " + tableName);
            }
            this.ProcessTable();
        }

        public DataTable GetTable()
        {
            return table;
        }

        private void ProcessTable()
        {


            bool exclude = this.excludeFirst;
            foreach (DataRow row in table.Rows)
            {
                if (exclude)
                {
                    exclude = false;
                    continue;
                }
                string column = (string)row["ColumnName"];
                Type type = (Type)row["DataType"];
                int columnSize = (int)row["ColumnSize"];
                MappedData data = new MappedData
                {
                    ColumnType = type,
                    MaxLength = columnSize
                };
                map.TryAdd(column, data);
            }
        }

        private DataTable LoadTable(string tableName)
        {
            DataTable schema = null;
            using (var connection = new MySqlConnection(MySQLHandler.GetLoginCredentials()))
            {
                using (var command = new MySql.Data.MySqlClient.MySqlCommand("SELECT * FROM " + tableName, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        schema = reader.GetSchemaTable();
                    }
                }
            }

            return schema;
        }

        public class MappedData
        {
            public Type ColumnType { get; set; }

            public int MaxLength { get; set; }

            public bool IsFloat()
            {
                return Type.GetTypeCode(ColumnType) == TypeCode.Single;
            }

            public bool IsInt()
            {
                return Type.GetTypeCode(ColumnType) == TypeCode.Int32;
            }

            public bool IsString()
            {
                return Type.GetTypeCode(ColumnType) == TypeCode.String;
            }

            public bool IsBoolean()
            {
                return Type.GetTypeCode(ColumnType) == TypeCode.Boolean;
            }

            public bool IsNumericType()
            {
                switch (Type.GetTypeCode(ColumnType))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return true;
                    default:
                        return false;
                }
            }
        }

    }
}
