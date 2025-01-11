using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string? ErrorMessage { get; private set; }

        public void OnGet()
        {
            ErrorMessage = TempData["errorMessage"] as string;
            if (!User.Identity.IsAuthenticated)
            {
                SetUserCookie();
            }
        }

        private void SetUserCookie()
        {
            var options = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,

            };
            
        }
    }
}
