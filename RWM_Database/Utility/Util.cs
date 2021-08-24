using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Utility
{
    public class Util
    {

    /* 
    * Class description: Utility functions
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static string ToTitleCase(string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }


       /* 
       * converts to cubic feet, (12 * 12 * 12)
       */

        public static float GetVolumeConversion()
        {
            return 1728;
        }

        public static string ReformatDate(string date)
        {

            try
            {
                return DateTime.Parse(date).ToString("MM-dd-yyyy");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not reformat date: " + ex.Message.ToString());
            }
            return date;
        }
    }
}
