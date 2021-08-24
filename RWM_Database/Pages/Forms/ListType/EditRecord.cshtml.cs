using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;
using RWM_Database.Utility;
using static RWM_Database.Backend.ListTypeHandler;

namespace RWM_Database.Pages.Forms.ListType
{


             /* 
    * Class description: Not currently implemented in the program. Needs work.
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
    public class EditRecordModel : PageModel
    {
        [BindProperty(Name = "ListName", SupportsGet = true)]
        public string TableName { get; set; }

        [BindProperty(Name = "ReferenceId", SupportsGet = true)]
        public int ReferenceId { get; set; }


        public string[] TableNames = new string[] { "items", "container", "shipment", "burial"};


        public MappedTable table;

        public PaginatedTable PaginatedTable;

        public ListTypeData ListTypeData;

        public IActionResult OnGet()
        {
            if (!TableNames.Contains(TableName))
            {
                return Page();
            }
            try
            {
                table = new MappedTable(TableName, true);

                List<ListTypeData> list = ListTypeHandler.LoadListTypeValues(TableName, table, " WHERE " + this.GetIdFieldFromTable(TableName) + " = " + ReferenceId);
                if (list.Count > 0)
                {
                    ListTypeData = list[0];
                }
                else throw new Exception("Tried to load an invalid record: " + ReferenceId);

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToPage("/Error", new { CustomError = ("Could not load record editor: " + ReferenceId) });
            }
        }

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {

            return RedirectToPage("ListDashboard", new { ListName = data["ListName"] });
        }

        public string FixOutput(string column)
        {
            string output = column.Replace("_", " ");
            output = Util.ToTitleCase(output);
            return output;
        }

        public string GetIdFieldFromTable(string table)
        {
            if (table == "items")
            {
                return "item_id";
            }
            else if (table == "container")
            {
                return "container_id";
            }
            else if (table == "shipment")
            {
                return "shipment_id";
            }
            else if (table == "burial")
            {
                return "burial_id";
            }
            else throw new Exception("Could not find table id field: " + ReferenceId);
        }
    }
}
