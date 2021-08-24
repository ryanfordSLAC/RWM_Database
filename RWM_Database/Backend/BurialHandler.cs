using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using RWM_Database.Backend.Attachments;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RWM_Database.Backend.Attachments.AttachmentHandler;

namespace RWM_Database.Backend
{
        /* 
    * Class description: Backend MySQL for the burial table
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
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

            public string DisposalSite { get; set; }

            public BurialData(int burialId, string burialNumber, string certificate, string disposalSite)
            {
                this.BurialId = burialId;
                this.BurialNumber = burialNumber;
                this.Certificate = certificate;
                this.DisposalSite = disposalSite;
            }
        }


        public void LoadBurialList()
        {

            BurialList = new List<BurialData>();
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();
                string baseCommand = "SELECT * FROM burial LEFT JOIN disposal_site_type ON burial.disposal_site_ref = disposal_site_type.disposal_site_id";
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
            int burialId = read.GetInt32("burial_id");
            string burialNumber = read.GetString("burial_number");
            string certificate = read.GetString("certificate");

            string disposalSite = "Invalid Disposal Site";

            if (MySQLHandler.ColumnExists(read, "disposal_site"))
            {
                if (!read.IsDBNull(read.GetOrdinal("disposal_site")))
                {
                    disposalSite = read.GetString("disposal_site");
                }
            }

            BurialData data = new BurialData(burialId, burialNumber, certificate, disposalSite);
            return data;
        }

        public static void CreateBurialDBEntry(IFormCollection data)
        {
            try
            {

                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO burial VALUES(0, @BurialNumber, @Certificate, @CertificateDate, @DisposalSite)");
                command.Parameters.AddWithValue("@BurialNumber", data["BurialNumber"]);
                command.Parameters.AddWithValue("@Certificate", data["Certificate"]);
                command.Parameters.AddWithValue("@CertificateDate", data["CertificateDate"]);
                command.Parameters.AddWithValue("@DisposalSite", data["DisposalSite"]);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
        //loads a burial from a given burial_id. attachments is an empty list that will populate with attachments loaded here
        public static BurialData LoadBurial(int burialId, List<AttachmentData> attachments)
        {
            BurialData data = null;
            try
            {
                MySqlConnection connection = MySQLHandler.GetMySQLConnection();
                MySqlCommand command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM burial LEFT JOIN attachments ON burial.burial_id = attachments.item_reference LEFT JOIN attachment_type ON attachments.type = attachment_type.attachment_type_id LEFT JOIN disposal_site_type ON burial.disposal_site_ref = disposal_site_type.disposal_site_id WHERE burial_id = @BurialId";
                command.Parameters.AddWithValue("@BurialId", burialId);
                MySqlDataReader read = command.ExecuteReader();
                if (read.HasRows)
                {
                    while (read.Read())
                    {
                        data = CreateBurialDataObject(read);

                        AttachmentData attachmentData = AttachmentHandler.CreateAttachmentObject(read);
                        if (attachmentData != null)
                        {
                            if (attachmentData.GetAttachmentTypeName() == Settings.GetStringSetting("Burial_Attachment_Type"))
                            {
                                attachments.Add(attachmentData);
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

            return data;
        }

    }
}
