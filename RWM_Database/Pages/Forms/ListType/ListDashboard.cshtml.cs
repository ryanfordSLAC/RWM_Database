using System;
using System.Collections.Generic;
using System.Data;
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
    public class ListTypeDashboardModel : PageModel
    {

        [BindProperty(Name = "ListName", SupportsGet = true)]
        public string ListName { get; set; }

        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }


        public string[] TableNames = new string[] { "container_type", "shipment_type", "people", "attachment_type", "seal_number_type", "disposal_site_type" };


        public MappedTable table;

        public PaginatedTable PaginatedTable;

        public List<ListTypeData> ListTypeData;

        public IActionResult OnGet()
        {
            if (!TableNames.Contains(ListName))
            {
                return Page();
            }
            try
            {
                table = new MappedTable(ListName, true);

                ListTypeData = ListTypeHandler.LoadListTypeValues(ListName, table, null);

                PaginatedTable = new PaginatedTable(10, table.GetTable().Rows.Count);

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToPage("/Error", new { CustomError = ("Could not load list type table: " + ListName) });
            }



        }

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {

            return RedirectToPage("ListDashboard", new { ListName = data["ListName"]});
        }

        public IActionResult OnPostAddButton(IFormCollection data, string listName)
        {
            try
            {
                MappedTable table = new MappedTable(listName, true);
                ListTypeHandler.CreateListTypeEntry(listName, data, table);
                return RedirectToPage("ListDashboard", new { ListName = listName });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return RedirectToPage("/Error", new { CustomError = ("Failed to create list type entry") });
            }
        }

        public string FixOutput(string column)
        {
            string output = column.Replace("_" , " ");
            output = Util.ToTitleCase(output);
            return output;
        }
    }
}
