using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.CRM;
using PIS_Web.Pages;
using System.Security.Claims;
using System.Text.Json;
using TF.MessageManagement.WebReference.SMSirTicketingWS;

[Authorize]
[Authorize(Roles = "Admin, User")]
public class TicketsModel : PageModel
{
    [BindProperty]
    public string SelectedSN { get; set; }
    [BindProperty(SupportsGet = true)]
    public int pageNumber { get; set; }
    public IEnumerable<TicketView>? Tickets { get; private set; }
    public List<Ticket>? Tickets2 { get; private set; }
    public string errorMessage { get; set; }
    public string priorityDs { get; set; }
    public string statusDs { get; set; }
    public string applicationDs { get; set; }

    private readonly DatabaseService _databaseService;
    private readonly ILogger<TicketsModel> _logger;
    private const int PageSize = 10;

    public int? TotalPages { get; set; }
    public TicketsModel(DatabaseService databaseService, ILogger<TicketsModel> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }
    public async Task OnGetAsync(int pageNumber = 1)
    {
        errorMessage = TempData["errorMessage"] as string;
        try
        {
            if (User.IsInRole("Admin"))
            {
                Tickets = await _databaseService.GetTicketsAsync(pageNumber, PageSize);
                if (Tickets.Any())
                {
                    TotalPages = (int)Math.Ceiling(Tickets.FirstOrDefault().TotalCount / (double)PageSize);
                    this.pageNumber = pageNumber;
                }
            }
            else if (User.IsInRole("User"))
            {
                Tickets = await _databaseService.GetUserTicketAsync(Convert.ToDecimal(User.FindFirstValue("UserSN")), pageNumber, PageSize);
                if (Tickets.Any())
                {
                    TotalPages = (int)Math.Ceiling(Tickets.FirstOrDefault().TotalCount / (double)PageSize);
                    this.pageNumber = pageNumber;
                }
            }
        }
        catch (Exception er)
        {
            _logger.LogError(er.Message);
            TempData["errorMessage"] = "خطا در دریافت اطلاعاعت";
            RedirectToPage("/TicketList");
        }
    }
    public async Task<IActionResult> OnPostDelete()
    {
        try
        {
            if (string.IsNullOrEmpty(SelectedSN))
            {
                TempData["errorMessage"] = "تیکتی انتخاب نشده";
                return RedirectToPage("/CRM/TicketList");
            }
            await _databaseService.DeleteTicketAsync(Convert.ToDecimal(SelectedSN));
            return RedirectToPage("/CRM/TicketList");
        }
        catch (Exception er)
        {
            TempData["errorMessage"] = er.Message;
            return RedirectToPage("/CRM/TicketList");
        }
    }
    public IActionResult OnPostEdit()
    {
        if (SelectedSN != null)
        {
            return RedirectToPage("/CRM/EditTicketForm", new { TicketSN = SelectedSN });
        }

        else
        {
            TempData["errorMessage"] = "هیچ تیکتی انتخاب نشده است.";
            return RedirectToPage();
        }
    }
    public async Task OnPostRefresh()
    {
        await OnGetAsync(pageNumber);
    }
    public IActionResult OnPostAttach()
    {
        if (SelectedSN != null)
        {
            return RedirectToPage("/CRM/AddTicketAttachment", new { TicketSN = SelectedSN });
        }
        else
        {
            TempData["errorMessage"] = "هیچ تیکتی انتخاب نشده است.";
            return RedirectToPage();
        }
    }
    public IActionResult OnPostView()
    {
        if (SelectedSN != null)
        {
            return RedirectToPage("/CRM/TicketInDetail", new { TicketSN = SelectedSN });
        }
        else
        {
            TempData["errorMessage"] = "هیچ تیکتی انتخاب نشده است.";
            return RedirectToPage();
        }
    }
}
