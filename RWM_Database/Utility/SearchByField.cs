using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RWM_Database.Pages.Forms.Shipment.ShipmentDashboardModel;

namespace RWM_Database.Utility
{
    public class SearchByField
    {
        public Dictionary<string, string> searchMap;


        public SearchByField()
        {
            searchMap = new Dictionary<string, string>();
        }

        public void AddSearch(string field, string data)
        {
            if (data == null)
            {
                return;
            }
            searchMap.TryAdd(field, data);
        }

        public Dictionary<string, string> GetSearchMap()
        {
            return this.searchMap;
        }

        public string GetSearch(string field)
        {
            searchMap.TryGetValue(field, out string value);
            return value;
        }

        public string GetURLSearch()
        {
            string output = "";
            foreach (string field in searchMap.Keys)
            {
                string value = this.GetSearch(field);
                if (value != null)
                {
                    output += "&" + field + "=" + value;
                }
            }
            return output;
        }

     }
}
