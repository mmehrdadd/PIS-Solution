using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.CRM;

namespace PIS_Web.Pages.CRM
{
    [Authorize]
    [Authorize(Roles = "Admin, User")]
    public class TicketDtlModel : PageModel
    {
        private readonly ILogger<TicketDtlModel> _logger;
        private readonly DatabaseService _dataBaseService;
        public string? errorMessage { get; set; }
        public IEnumerable<TicketDtl>? TicketDtls { get; set; }
        [BindProperty]
        public string? selectedSN { get; set; }

        public TicketDtlModel(DatabaseService dataBaseService, ILogger<TicketDtlModel> logger)
        {
            _dataBaseService = dataBaseService;
            _logger = logger;
        }
        public async Task OnGet()
        {
            errorMessage = TempData["errorMessage"] as string;
            try
            {
                TicketDtls = await _dataBaseService.GetTicketDtlListAsync();
            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "خطا در دریافت اطلاعات";
                RedirectToPage();
            }

        }
        public IActionResult OnPostAddAsync()
        {

            return RedirectToPage("/CRM/AddTicketDtl");
        }
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (selectedSN != null)
            {
                try
                {
                    await _dataBaseService.DeleteTicketDtlAsync(Convert.ToDecimal(selectedSN));
                    return RedirectToPage();
                }
                catch (Exception)
                {
                    TempData["errorMessage"] = "شماره تیکت اشتباه است";
                    return RedirectToPage();
                }
                
            }
            TempData["errorMessage"]= "تیکتی انتخاب نشده است";
            return RedirectToPage();
        }
    }
}
