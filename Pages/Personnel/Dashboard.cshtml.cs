using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.Personnel;
using System.Security.Claims;
using PIS.Person;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace PIS_Web.Pages.Personnel
{
    [Authorize]
    [Authorize(Roles = "Admin , User")]
    public class DashboardModel : PageModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<DashboardModel> _logger;
        [BindProperty]
        public Person Person { get; set; }
        public PersonAttachment PersonAttachment { get; set; }

        public DashboardModel(DatabaseService databaseService, ILogger<DashboardModel> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task OnGet()
        {
            try
            {
                Person = await _databaseService.GetPersonInfoById(Convert.ToDecimal(User.FindFirstValue("UserSN")));
                //var personAvatar =  PIS.Person.cPerson.GetPersonAvatar(SM, Convert.ToDecimal(User.FindFirstValue("UserSN")));
                Person.RegDate = AddDateSlash(Person.RegDate);
            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "خطا در دریافت اطلاعات";
                RedirectToPage();
            }
        }

        private string? AddDateSlash(string? regDate)
        {
            var formattedDate = regDate;
            if (!string.IsNullOrEmpty(regDate))
            {
                 formattedDate = Regex.Replace(regDate, @"(\d{4})(\d{2})(\d{2})", "$1/$2/$3");
            }
            return formattedDate;
        }

        public async Task<IActionResult> OnGetImageAsync()
        {
            try
            {
                Person = await _databaseService.GetPersonInfoById(Convert.ToDecimal(User.FindFirstValue("UserSN")));
                
                if (!Person.PersonSN.Equals(null))
                {
                    PersonAttachment = await _databaseService.GetPersonAvatar(Person.PersonSN);
                }
                
                if (PersonAttachment.FileSave == null)
                {
                    return NotFound();
                }
                return File(PersonAttachment.FileSave, $"image/{PersonAttachment.FileExtention}");
            }
            catch (Exception)
            {

                return NotFound();
            }
           

            
        }
    }
}
