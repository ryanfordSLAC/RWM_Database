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
 * This class creates containers in the database
 * from user form input
 * Author: James Meadows
*/

namespace RWM_Database.Pages.Forms
{
    public class CreateContainerModel : PageModel
    {
        public Dictionary<int, string> containerTypeMap;
        public List<PeopleData> people;
        public void OnGet()
        {
            containerTypeMap = ListTypeHandler.GetIdMap("container_type");
            people = PeopleHandler.LoadPeopleCondition(null);
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

            ContainerHandler.CreateContainer(data);
            return RedirectToPage("ContainerDashboard");
        }

    }
}
