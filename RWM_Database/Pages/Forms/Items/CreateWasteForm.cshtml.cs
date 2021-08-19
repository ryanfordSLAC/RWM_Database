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

namespace RWM_Database.Pages.Forms
{
    public class CreateWasteFormModel : PageModel
    {

        public PackedContainerHandler ContainerHandler { get; set; }

        public List<PeopleData> people;

        public void OnGet()
        {
            ContainerHandler = new PackedContainerHandler(-1, -1);
            people = PeopleHandler.LoadPeopleCondition(null);
        }

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            if (!data.ContainsKey("ContainerId"))
            {
                return RedirectToPage("/Error", new { CustomError = ("Invalid container provided") });
            }
            this.CreateWasteDeclarationItem(data);
            return RedirectToPage("WasteDeclarationForm"); //item created success page
        }



        /*
        Creates the waste declaration form entry in the MySQL Database
        */



        private void CreateWasteDeclarationItem(IFormCollection data)
        {
            try
            {
                int containerId = -1;
                if (data.ContainsKey("ContainerId"))
                {
                    containerId = Convert.ToInt32(data["ContainerId"]);
                }

                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO items VALUES(0, @DeclarationNumber, @ContainerId, @ItemDescription, @Location, @AccountNumber, @HazardousMaterial, @GeneratorId, @GenerationDate, @RecievedBy, @RecievedDate, @SealedSource, @Length, @Width, @Height, @UserID)");
                command.Parameters.AddWithValue("@DeclarationNumber", data["DeclarationNumber"]);
                command.Parameters.AddWithValue("@ContainerId", containerId);
                command.Parameters.AddWithValue("@ItemDescription", data["ItemDescription"]);
                command.Parameters.AddWithValue("@Location", data["Location"]);
                command.Parameters.AddWithValue("@AccountNumber", data["AccountNumber"]);
                command.Parameters.AddWithValue("@HazardousMaterial", data["HazardousMaterial"]);
                command.Parameters.AddWithValue("@GeneratorId", data["GeneratorId"]);
                command.Parameters.AddWithValue("@GenerationDate", data["GenerationDate"]);
                command.Parameters.AddWithValue("@RecievedBy", data["RecievedBy"]);
                command.Parameters.AddWithValue("@RecievedDate", data["RecievedDate"]);
                command.Parameters.AddWithValue("@SealedSource", data["SealedSource"]);
                command.Parameters.AddWithValue("@Length", (float)Convert.ToDouble(data["Length"]));
                command.Parameters.AddWithValue("@Width", (float)Convert.ToDouble(data["Width"]));
                command.Parameters.AddWithValue("@Height", (float)Convert.ToDouble(data["Height"]));
                command.Parameters.AddWithValue("@UserID", -1);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

    }
}
