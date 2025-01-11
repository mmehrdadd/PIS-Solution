using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS_Web.Pages
{
    public class DataBaseErrorModel : PageModel
    {
        public string? ErrorMessage { get; set; }
        public void OnGet()
        {
            ErrorMessage = TempData["errorMessage"] as string;
        }
    }
}
