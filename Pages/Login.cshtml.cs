using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.CRM;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TF.SecurityManager;
using TF.SecuritySystem;
using static System.Net.Mime.MediaTypeNames;



namespace PIS_Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<LoginModel> _logger;
        public LoginModel(DatabaseService databaseService, ILogger<LoginModel> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }
        [BindProperty]
        public long UserId { get; set; }

        [BindProperty]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }
        public Model.CRM.User USER { get; set; }


        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.Response.Redirect("Index");
            }
            ErrorMessage = TempData["errorMessage"] as string;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            try
                {
                USER = await _databaseService.GetPassAsync(UserId);
                var decodedPass = TF.CommonComponent.CCommonFunction.DecodeString(USER.UserPassword);
                if (USER.UserName.Contains('-'))
                {
                    USER.UserName = USER.UserName.Replace(" -", "");
                }

                if (decodedPass != Password)
                {
                    
                    TempData["errorMessage"] = "نام کاربری یا رمز عبور اشتباه است.";
                    return RedirectToPage();
                }
                
                //var iniFilePath = Path.Combine(binPath, "StartInfo.ini");
                
                var claims = new List<Claim>{

                    new (ClaimTypes.Name, USER.UserName),
                    new (ClaimTypes.Role, USER.IsAdmin ? "Admin":"User"),
                    new ("UserSN",USER.UserSN)

                };
                if (USER.IsExpert)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Expert"));
                }
                var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimIdentity),
                authProperties);
                ErrorMessage = "Login success";
                
                return RedirectToPage("/Personnel/Dashboard");
            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "نام کاربری یا رمز عبور اشتباه است.";
                return RedirectToPage();
            }
        }
    }
}


