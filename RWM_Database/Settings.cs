using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database
{

    public class Settings
    {

        private static Dictionary<string, string> ProgramSettings;

        public static int GetIntegerSetting(string key)
        {
            return Convert.ToInt32(ProgramSettings[key]);
        }

        public static string GetStringSetting(string key)
        {
            return ProgramSettings[key];
        }

        public static void LoadSettings(Dictionary<string, string> programSetting)
        {
            ProgramSettings = programSetting;
        }
    }
}
