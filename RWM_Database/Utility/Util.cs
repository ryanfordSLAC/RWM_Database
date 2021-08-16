using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Utility
{
    public class Util
    {

        public static string GetCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
