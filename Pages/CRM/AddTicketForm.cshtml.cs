using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography.Xml;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using TF.MessageManagement.WebReference.SMSirTicketingWS;
using System.Net.Sockets;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using PIS_Web.Model.CRM;

namespace PIS_Web.Pages.CRM
{
    [Authorize]
    [Authorize(Roles = "Admin,User")]
    public class AddTicketForm : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public decimal? ticketSN { get; set; }
        public Ticket newTicket { get; set; }
        [BindProperty]
        public Ticket Ticket { get; set; }
        [BindProperty]
        public Ticket inputTicket { get; set; }
        [BindProperty]
        public string PersianDateInput { get; set; }
        [BindProperty]
        public IFormFile Attachments { get; set; }
        [BindProperty]
        public IList<IFormFile> UploadedFiles { get; set; }
        private readonly DatabaseService _databaseService;
        private readonly ILogger<AddTicketForm> _logger;
        public IEnumerable<GeneralStatus> priorities { get; private set; }
        public IEnumerable<GeneralStatus> recordStatus { get; private set; }
        public IEnumerable<Problem> problem { get; private set; }
        public IEnumerable<Ticket> refrences { get; private set; }
        public IEnumerable<seApplication> subSystems { get; private set; }
        public IEnumerable<GeneralStatus> ticketType { get; private set; }
        public DateTime gregorianToday { get; set; } = DateTime.Today;
        PersianCalendar shamsiToday { get; set; } = new PersianCalendar();
        public string ticketDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? errorMessage { get; private set; }

        public AddTicketForm(DatabaseService databaseService, ILogger<AddTicketForm> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }
        public async Task OnGetAsync(decimal? ticketSN)
        {
            errorMessage = TempData["errorMessage"] as string;
            if (ticketSN == null)
            {
                try
                {
                    //refrences = await _databaseService.GetTicketsAsync();
                    priorities = await _databaseService.GetPriorityAsync();
                    subSystems = await _databaseService.GetSeApplicationsAsync();
                    recordStatus = await _databaseService.GetRecordStatusAsync();
                    ticketType = await _databaseService.GetTicketTypeAsync();
                    problem = await _databaseService.GetProblemListAsync();
                    ticketDate = ConvertToPersianDate(gregorianToday, shamsiToday, false);

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
                    //refrences = await _databaseService.GetTicketsAsync();
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
                return RedirectToPage();
            }
            var hours = DateTime.Now.ToString("hh:mm:ss");
            var timeInSeconds = TimeSpan.Parse(hours).TotalSeconds;
            try
            {
                newTicket = new Ticket
                {
                    FatherTicketSN = inputTicket.FatherTicketSN,
                    TicketDs = inputTicket.TicketDs,
                    TicketUserSN = Convert.ToDecimal(User.FindFirstValue("UserSN")),
                    TicketDate = ConvertToPersianDate(gregorianToday, shamsiToday, true),
                    TicketTime = timeInSeconds.ToString(),
                    IsSupervisorConfirm = false,
                    TicketTypeSN = inputTicket.TicketTypeSN,
                    PrioritySN = inputTicket.PrioritySN,
                    StatusSN = inputTicket.StatusSN,
                    IsClosed = false,
                    ApplicationSN = inputTicket.ApplicationSN,
                    ProblemSN = inputTicket.ProblemSN,
                    DefaultLanguageSN = inputTicket.DefaultLanguageSN == null ? 74 : inputTicket.DefaultLanguageSN
                };
                await _databaseService.AddTicketAsync(newTicket);
                return RedirectToPage("/CRM/TicketList");
            }
            catch (Exception er)
            {
                TempData["errorMessage"] = "خطا در بارگذاری تیکت";
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
        public IActionResult OnPostReset()
        {
            return RedirectToPage("/Index");
        }
    }
}
