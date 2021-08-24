using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RWM_Database.Backend;
using RWM_Database.Utility;

namespace RWM_Database.Pages.Forms.Container
{

    /* 
    * Class description: Container dashboard which shows a table of existing containers
    * 
    * Author: James Meadows
    * Intern at SLAC during summer of 2021
    * For questions contact by email at: jamesmeadows18@outlook.com
    */
    public class ContainerDashboardModel : PageModel
    {
        public ContainerHandler ContainerHandler { get; set; }

        //search table by container_number
        [BindProperty(Name = "SearchContainerNumber", SupportsGet = true)]
        public string SearchContainerNumber { get; set; }

        //search table by shipment number
        [BindProperty(Name = "SearchShipmentNumber", SupportsGet = true)]
        public string SearchShipmentNumber { get; set; }

        //table pagination
        [BindProperty(Name = "CurrentPage", SupportsGet = true)]
        public int CurrentPage { get; set; }

        public PaginatedTable PaginatedTable { get; set; }

        public void OnGet()
        {

            ContainerHandler = new ContainerHandler();
            SearchByField search = ContainerHandler.Search;

            search.AddSearch("container_number", SearchContainerNumber);
            search.AddSearch("shipment_number", SearchShipmentNumber);

            ContainerHandler.LoadContainers();

            PaginatedTable = new PaginatedTable(10, ContainerHandler.ContainerList.Count);


        }

        public IActionResult OnPostSearchButton(IFormCollection data)
        {

            return RedirectToPage("ContainerDashboard", new { SearchContainerNumber = data["SearchContainerNumber"], SearchShipmentNumber = data["SearchShipmentNumber"] });
        }

        public IActionResult OnPostClearButton(IFormCollection data)
        {
            return RedirectToPage("ContainerDashboard");
        }

        public IActionResult OnPostCreateButton()
        {

            return RedirectToPage("CreateContainer");
        }
    }
}
