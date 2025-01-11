using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.CRM;
using System.Security.Claims;

namespace PIS_Web.Pages.CRM
{
    [Authorize]
    [Authorize(Roles = "Admin, User")]
    public class AddTicketDtlModel : PageModel
    {
        private readonly DatabaseService _databaseService;
        [BindProperty]
        public TicketDtl TicketDtl { get; set; }
        public string? errorMessage { get; set; }
        public IEnumerable<Ticket>? Tickets { get; set; }
        public IEnumerable<TicketView>? TicketViews { get; set; }
        public IEnumerable<seUser?> Experts { get; set; }

        public AddTicketDtlModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        public async Task OnGet()
        {
            errorMessage = TempData["errorMessage"] as string;
            try
            {
                if (User.IsInRole("Admin"))
                {
                    Tickets = await _databaseService.GetTicketsAsync();
                }
                else { TicketViews = await _databaseService.GetUserTicketAsync(Convert.ToDecimal(User.FindFirstValue("UserSN"))); }
                Experts = await _databaseService.GetExpertsAsync();

            }
            catch (Exception er)
            {
                TempData["errorMessage"] = "خطا در دریافت اطلاعات";
                RedirectToPage("/CRM/TicketDtl");

            }
        }
        public async Task<IActionResult> OnPost()
        {
            try
            {
                var newTicketDtl = new TicketDtl
                {
                    TicketSN = TicketDtl.TicketSN,
                    ExpertUserSN = TicketDtl.ExpertUserSN,
                    MessageBody = TicketDtl.MessageBody,
                    DefaultLanguageSN = TicketDtl.DefaultLanguageSN == null ? 74 : TicketDtl.DefaultLanguageSN
                };
                await _databaseService.AddTicketDtlAsync(newTicketDtl);
                return RedirectToPage("/CRM/TicketDtl");
            }
            catch (Exception er)
            {
                TempData["errorMessage"] = "خطا در ساخت پیام جدید";
                return RedirectToPage();
            }

        }
    }
}
