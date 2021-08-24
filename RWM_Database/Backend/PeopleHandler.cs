using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Backend
{


         /* 
    * Class description: Backend MySQL for people table
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class PeopleHandler
    {

        public class PeopleData
        {


            public int PeopleId { get; set; }

            public string SlacId{ get; set; }

            public string LastName { get; set; }

            public string FirstName { get; set; }

            public bool Admin { get; set; }

            public PeopleData(int peopleId, string slacId, string lastName, string firstName, bool admin)
            {
                this.PeopleId = peopleId;
                this.SlacId = slacId;
                this.LastName = lastName;
                this.FirstName = firstName;
            }
        }

        public static PeopleData CreatePersonDataObject(MySqlDataReader read)
        {
            int peopleId = read.GetInt32("people_id");
            string slacId = read.GetString("slac_id");
            string lastName = read.GetString("last_name");
            string firstName = read.GetString("first_name");
            bool admin = read.GetBoolean("admin");

            PeopleData data = new PeopleData(peopleId, slacId, lastName, firstName, admin);

            return data;
        }


        public static List<PeopleData> LoadPeople(Action<MySqlCommand> onCreate)
        {
            List<PeopleData> list = new List<PeopleData>();

            void onRead(MySqlDataReader read)
            {
                PeopleData data = CreatePersonDataObject(read);
                list.Add(data);
            }

            MySQLHandler.ReadFromDatabase(onCreate, onRead);

            return list;
        }

        public static List<PeopleData> LoadPeopleCondition(string condition)
        {
            string baseCommand = "SELECT * FROM people";

            void onCreate(MySqlCommand command)
            {
                if (condition != null)
                {
                    baseCommand += " " + condition;
                }
                command.CommandText = baseCommand;
            }

            List<PeopleData> list = LoadPeople(onCreate);

            return list;
        }
    }
}
