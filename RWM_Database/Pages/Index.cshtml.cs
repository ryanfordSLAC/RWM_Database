using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RWM_Database.Backend.Reports;
using RWM_Database.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Pages
{

            /* 
    * Class description: Main dashboard menu. Shows basic report information
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */

    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public ItemReport ItemReport { get; set; }

        public PackedContainerHandler ContainerHandler { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            ItemReport = new ItemReport();
            ContainerHandler = new PackedContainerHandler(-1, -1);
        }

        public IActionResult OnPostViewButton(IFormCollection data)
        {
            return RedirectToPage("/Forms/Reports/ViewReport", new { ReportType = data["ReportType"], StartDate = data["StartDate"], EndDate = data["EndDate"] });
        }

        public IActionResult OnPostExportButton(IFormCollection data)
        {
            this.FullDatabaseExport();
            return RedirectToPage("/Forms/Reports/Download", new { FilePath = ("RWM_Database" + ".xlsx") });
        }

        private void FullDatabaseExport()
        {

            DataTable table = TableReport.ReadAllItems("SELECT * FROM items " +
                "LEFT JOIN people ON items.generator_id = people.people_id " +
                "LEFT JOIN container ON items.container_ref = container.container_id " +
                "LEFT JOIN container_type ON container.type_ref = container_type.container_type_id " +
                "LEFT JOIN seal_number_type ON container.seal_number_id = seal_number_type.seal_number_id " +
                "LEFT JOIN shipment ON container.shipment_ref = shipment.shipment_id " +
                "LEFT JOIN shipment_type ON shipment.shipment_type_ref = shipment_type.shipment_type_id " +
                "LEFT JOIN burial ON shipment.burial_ref = burial.burial_id "

            );


            //these mysql columns will not be displayed in the Excel output
            List<string> omit = new List<string>();
            omit.Add("item_id");
            omit.Add("container_ref");
            omit.Add("generator_id");
            omit.Add("recieved_by");
            omit.Add("sealed_source");
            omit.Add("user_ref");
            omit.Add("container_id");
            omit.Add("seal_number");
            omit.Add("type_ref");
            omit.Add("packed_by");
            omit.Add("shipment_ref");
            omit.Add("user_ref1");
            omit.Add("seal_number_id");
            omit.Add("container_type_id");
            omit.Add("shipment_id");
            omit.Add("shipment_type_ref");
            omit.Add("burial_ref");
            omit.Add("seal_number_id1");
            omit.Add("people_id");
            omit.Add("burial_id");
            omit.Add("disposal_site_ref");
            omit.Add("shipment_type_id");
            string folder = Directory.GetCurrentDirectory() + "/wwwroot/Export/";
            System.IO.Directory.CreateDirectory(folder);
            ExportExcel.ExportTable(table, folder + "RWM_Database" + ".xlsx", omit);
        }

    }
}
