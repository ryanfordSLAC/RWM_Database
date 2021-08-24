using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using RWM_Database.Backend;
using static RWM_Database.Backend.PeopleHandler;
using static RWM_Database.Pages.Forms.ViewContainerModel;

/* 
* Class description: Adds container to the database in the container table
* 
* Author: James Meadows
* Intern at SLAC during summer of 2021
* For questions contact by email at: jamesmeadows18@outlook.com
*/

namespace RWM_Database.Pages.Forms
{
    public class CreateContainerModel : PageModel
    {
        //convert container type id to container type name
        public Dictionary<int, string> containerTypeMap;

        //convert seal id to seal number
        public Dictionary<int, string> sealTypeMap;

        //convert people id to LastName FirstName
        public List<PeopleData> people;
        public void OnGet()
        {
            containerTypeMap = ListTypeHandler.GetIdMap("container_type");
            sealTypeMap = ListTypeHandler.GetIdMap("seal_number_type");
            people = PeopleHandler.LoadPeopleCondition(null); //null means no WHERE clause for mysql search
        }

        /*
        When the Submit Button is pressed: create an entry in the database for the desired
        * container with the input data
        */

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {

            if (!data.ContainsKey("TypeId"))
            {
                return RedirectToPage("/Error", new { CustomError = ("Invalid container type provided. Please select a container type or create a new one") });
            }

            if (!data.ContainsKey("SealNumber"))
            {
                return RedirectToPage("/Error", new { CustomError = ("Invalid seal number provided. Please select a container type or create a new one") });
            }

            ContainerHandler.CreateContainer(data);
            return RedirectToPage("ContainerDashboard");
        }

    }
}
