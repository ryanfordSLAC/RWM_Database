using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RWM_Database.Backend.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RWM_Database.Pages
{
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

    }
}
