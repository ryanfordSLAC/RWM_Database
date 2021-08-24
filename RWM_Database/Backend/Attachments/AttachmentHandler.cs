using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Backend.Attachments
{
    /* 
    * Class description: MySQL backend attachments used by each item, container, shipment, and burial view
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
    public class AttachmentHandler
    {


        public class AttachmentData
        {

            readonly string name;
            readonly int id;
            readonly byte[] data;
            readonly string date;
            readonly string creator;
            readonly int type;

            readonly string attachmentType;

            public AttachmentData(int id, string name, byte[] data, string date, string creator, int type, string attachmentType)
            {
                this.id = id;
                this.name = name;
                this.data = data;
                this.date = date;
                this.creator = creator;
                this.type = type;
                this.attachmentType = attachmentType;
            }

            public string GetAttachmentName()
            {
                return name;
            }

            public int GetAttachmentId()
            {
                return id;
            }

            public byte[] GetData()
            {
                return this.data;
            }

            public string GetAttachmentDate()
            {
                return date;
            }

            public int GetAttachmentType()
            {
                return type;
            }

            public string GetAttachmentTypeName()
            {
                return attachmentType;
            }

            public string GetAttachmentCreator()
            {
                return creator;
            }
        }


        /*
        * Converts attachment file to byte[]
        * Then uploads to the mysql server
        */
        public static  void UploadFileToDB(IFormFile file, int itemReference, int attachmentType, string accountName)
        {
            try
            {
                //file to byte[]
                using var fileStream = file.OpenReadStream();
                byte[] buffer = new byte[file.Length];
                fileStream.Read(buffer, 0, (int)file.Length);

                //Upload the file to the database
                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO attachments VALUES(0, @ItemReference, @FileName, @Type, @Data, @Date, @Account_name)");
                command.Parameters.AddWithValue("@ItemReference", itemReference);
                command.Parameters.AddWithValue("@FileName", file.FileName);
                command.Parameters.AddWithValue("@Type", attachmentType);
                command.Parameters.AddWithValue("@Data", buffer);
                command.Parameters.AddWithValue("@Date", DateTime.Now.ToString("MM-dd-yyyy"));
                command.Parameters.AddWithValue("@Account_name", accountName);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static AttachmentData CreateAttachmentObject(MySqlDataReader read)
        {
            if (!read.IsDBNull(read.GetOrdinal("file_name")))
            {
                int id = read.GetInt32("attachment_id");
                string fileName = read.GetString("file_name");
                int typeId = read.GetInt32("type");
                byte[] byte_data = (byte[])read["data"];
                string date = read.GetString("date");
                string account_name = read.GetString("account_name");

                string attachmentType = "Invalid Attachment Type";

                if (MySQLHandler.ColumnExists(read, "document_type"))
                {
                    if (!read.IsDBNull(read.GetOrdinal("document_type")))
                    {
                        attachmentType = read.GetString("document_type");
                    }
                }


                return new AttachmentData(id, fileName, byte_data, date, account_name, typeId, attachmentType);
            }
            return null;
        }

    }
}
