using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using RWM_Database.Backend;
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
        public void OnGet()
        {
            containerTypeMap = ListTypeHandler.GetIdMap("container_type");
        }

        /*
        When the Submit Button is pressed: create an entry in the database for the desired
        * container with the input data
        */

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            ContainerHandler.CreateContainer(data);
            return RedirectToPage("ContainerDashboard");
        }

    }
}
