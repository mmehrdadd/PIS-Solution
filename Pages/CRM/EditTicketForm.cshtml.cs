using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.CRM;
using System.Globalization;
using System.Security.Claims;

namespace PIS_Web.Pages.CRM
{
    [Authorize]
    [Authorize(Roles = "Admin,User")]
    public class EditTicketFormModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public decimal ticketSN { get; set; }
        public Ticket newTicket { get; set; }
        [BindProperty]
        public Ticket Ticket { get; set; }
        [BindProperty]
        public Ticket inputTicket { get; set; }
        [BindProperty]
        public string PersianDateInput { get; set; }
        [BindProperty]
        public IFormFile Attachments { get; set; }
        private readonly DatabaseService _databaseService;
        private readonly ILogger<EditTicketFormModel> _logger;
        public IEnumerable<GeneralStatus> priorities { get; private set; }
        public IEnumerable<GeneralStatus> recordStatus { get; private set; }
        public IEnumerable<Ticket> refrences { get; private set; }
        public IEnumerable<Problem> problem { get; private set; }
        public IEnumerable<seApplication> subSystems { get; private set; }
        public IEnumerable<GeneralStatus> ticketType { get; private set; }
        public DateTime gregorianToday { get; set; } = DateTime.Today;
        PersianCalendar shamsiToday { get; set; } = new PersianCalendar();
        public string ticketDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? errorMessage { get; private set; }

        public EditTicketFormModel(DatabaseService databaseService, ILogger<EditTicketFormModel> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }
        public async Task OnGetAsync(decimal ticketSN)
        {
            errorMessage = TempData["errorMessage"] as string;

            if (ticketSN.Equals(null))
            {
                try
                {
                    refrences = await _databaseService.GetTicketsAsync();
                    priorities = await _databaseService.GetPriorityAsync();
                    subSystems = await _databaseService.GetSeApplicationsAsync();
                    recordStatus = await _databaseService.GetRecordStatusAsync();
                    ticketType = await _databaseService.GetTicketTypeAsync();
                    ticketDate = ConvertToPersianDate(gregorianToday, shamsiToday, false);
                    problem = await _databaseService.GetProblemListAsync();
                }
                catch (Exception er)
                {
                    TempData["errorMessage"] = "خطا در دریافت اطلاعات";
                    RedirectToPage();
                }
            }
            else
            {
                try
                {
                    this.ticketSN = ticketSN;
                    Ticket = await _databaseService.GetTicketBySN_InsertAsync(ticketSN);
                    ticketDate = Ticket.TicketDate;
                    refrences = await _databaseService.GetTicketsAsync();
                    priorities = await _databaseService.GetPriorityAsync();
                    subSystems = await _databaseService.GetSeApplicationsAsync();
                    recordStatus = await _databaseService.GetRecordStatusAsync();
                    ticketType = await _databaseService.GetTicketTypeAsync();
                    problem = await _databaseService.GetProblemListAsync();
                }
                catch (Exception er)
                {
                    TempData["errorMessage"] = "خطا در دریافت اطلاعات";
                    RedirectToPage();
                }

                //ticketDate = ConvertToPersianDate(gregorianToday, shamsiToday, false);
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (inputTicket.TicketDs == null)
            {
                TempData["errorMessage"] = "شرح تیکت وارد نشده است.";
                RedirectToPage();
            }
            var hours = DateTime.Now.ToString("hh:mm:ss");
            var timeInSeconds = TimeSpan.Parse(hours).TotalSeconds;
            try
            {
                newTicket = new Ticket
                {
                    TicketSN = ticketSN,
                    FatherTicketSN = inputTicket.FatherTicketSN,
                    TicketDs = inputTicket.TicketDs,
                    TicketUserSN = Convert.ToDecimal(User.FindFirstValue("UserSN")),
                    IsSupervisorConfirm = false,
                    TicketTypeSN = inputTicket.TicketTypeSN,
                    PrioritySN = inputTicket.PrioritySN,
                    StatusSN = inputTicket.StatusSN,
                    IsClosed = false,
                    ApplicationSN = inputTicket.ApplicationSN,
                    DefaultLanguageSN = inputTicket.DefaultLanguageSN == null ? 74 : inputTicket.DefaultLanguageSN
                };
                if (Attachments != null && Attachments.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Attachments.CopyToAsync(memoryStream);
                        byte[] fileBytes = memoryStream.ToArray();


                        // await _databaseService.SaveAttachmentForTicketAsync(newTicket.TicketSN, Attachments.FileName, fileBytes);
                    }
                }
                await _databaseService.UpdateTicketAsync(newTicket);
                return RedirectToPage("/CRM/TicketList", new { TicketSN = ticketSN });
            }
            catch (Exception er)
            {
                TempData["errorMessage"] = "خطا در ثبت اطلاعات";
                return RedirectToPage();
            }

        }
        public static string ConvertToPersianDate(DateTime gregorianDate, PersianCalendar persianCalendar, bool isTrimmed)
        {
            int year = persianCalendar.GetYear(gregorianDate);
            int month = persianCalendar.GetMonth(gregorianDate);
            int day = persianCalendar.GetDayOfMonth(gregorianDate);
            if (isTrimmed)
            {
                return $"{year}{month:D2}{day:D2}";
            }
            return $"{year}/{month:D2}/{day:D2}";
        }

        public IActionResult OnPostAttach()
        {
            if (!ticketSN.Equals(null))
            {
                return RedirectToPage("/CRM/AddTicketAttachment", new { TicketSN = ticketSN });
            }
            else
            {
                return Page();
            }
        }
        public IActionResult OnPostReset()
        {
            return RedirectToPage("/CRM/AddTicketForm");
        }
    }
}


