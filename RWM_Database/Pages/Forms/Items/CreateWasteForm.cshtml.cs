using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace RWM_Database.Pages.Forms
{
    public class CreateWasteFormModel : PageModel
    {

        public PackedContainerHandler ContainerHandler { get; set; }



        public void OnGet()
        {
            ContainerHandler = new PackedContainerHandler(null);
        }

        public IActionResult OnPostSubmitButton(IFormCollection data)
        {
            this.CreateWasteDeclarationItem(data);
            return RedirectToPage("PreviewWasteDeclarationForm", new { DeclarationNumber = data["DeclarationNumber"] });
        }



        /*
        Creates the waste declaration form entry in the MySQL Database
        */



        private void CreateWasteDeclarationItem(IFormCollection data)
        {
            try
            {
                MySqlCommand command = MySQLHandler.GetMySQLConnection().CreateCommand();
                command.CommandText = ("INSERT INTO items VALUES(0, @DeclarationNumber, @ContainerNumber, @ItemDescription, @Location, @AccountNumber, @HazardousMaterial, @GeneratorName, @GenerationDate, @RecievedBy, @RecievedDate, @Length, @Width, @Height)");
                command.Parameters.AddWithValue("@DeclarationNumber", data["DeclarationNumber"]);
                command.Parameters.AddWithValue("@ContainerNumber", data["ContainerNumber"]);
                command.Parameters.AddWithValue("@ItemDescription", data["ItemDescription"]);
                command.Parameters.AddWithValue("@Location", data["Location"]);
                command.Parameters.AddWithValue("@AccountNumber", data["AccountNumber"]);
                command.Parameters.AddWithValue("@HazardousMaterial", data["HazardousMaterial"]);
                command.Parameters.AddWithValue("@GeneratorName", data["GeneratorName"]);
                command.Parameters.AddWithValue("@GenerationDate", data["GenerationDate"]);
                command.Parameters.AddWithValue("@RecievedBy", data["RecievedBy"]);
                command.Parameters.AddWithValue("@RecievedDate", data["RecievedDate"]);
                command.Parameters.AddWithValue("@Length", (float)Convert.ToDouble(data["Length"]));
                command.Parameters.AddWithValue("@Width", (float)Convert.ToDouble(data["Width"]));
                command.Parameters.AddWithValue("@Height", (float)Convert.ToDouble(data["Height"]));
                Console.WriteLine(command.CommandText);
                command.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

    }
}
