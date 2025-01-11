using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.CRM;
using System.Globalization;
using System.Security.Claims;
using TF.MessageManagement.WebReference.SMSirTicketingWS;

namespace PIS_Web.Pages.CRM
{
    [Authorize]
    [Authorize(Roles = "Admin, User")]
    public class TicketInDetailModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public decimal ticketSN { get; set; }
        [BindProperty]
        public Ticket Ticket { get; set; }
        [BindProperty]
        public string PersianDateInput { get; set; }
        [BindProperty]
        public IEnumerable<TicketAttachment> Attachments { get; set; }
        [BindProperty]
        public IEnumerable<TicketAttachment> TicketDtlAttachments { get; set; }
        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        [BindProperty]
        public IEnumerable<TicketDtl> TicketDtls { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? errorMessage { get; private set; }
        [BindProperty]
        public TicketDtl TicketDtl { get; set; }
        [BindProperty]
        public TicketAttachment TicketAttachment { get; set; }


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
        


        public TicketInDetailModel(ILogger<EditTicketFormModel> logger,DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            errorMessage = TempData["errorMessage"] as string;

            if (ticketSN.Equals(null))
            {
                throw new Exception();
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
                    TicketDtls = await _databaseService.GetTicketDtlAsync(ticketSN);
                    Attachments = await _databaseService.GetTicketAttachmentsAsync(ticketSN);
                }
                catch (Exception er)
                {
                    TempData["errorMessage"] = "خطا در دریافت اطلاعات";
                    RedirectToPage("/CRM/TicketInDetail");
                }

                //ticketDate = ConvertToPersianDate(gregorianToday, shamsiToday, false);
            }
        }
        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (!string.IsNullOrEmpty(TicketDtl.MessageBody))
                {
                
                    var newTicketDtl = new TicketDtl
                    {
                        TicketSN = this.ticketSN,
                        ExpertUserSN = TicketDtl.ExpertUserSN == 0 ? null : TicketDtl.ExpertUserSN,
                        MessageBody = TicketDtl.MessageBody,
                        DefaultLanguageSN = TicketDtl.DefaultLanguageSN == null ? 74 : TicketDtl.DefaultLanguageSN,
                        UserID_Name = User.Identity.Name

                    };
                    
                    await _databaseService.AddTicketDtlAsync(newTicketDtl);
                    if (UploadedFile != null)
                    {
                       await AddTicketDtlAttachment(UploadedFile);
                    }
                    return RedirectToPage("/CRM/TicketInDetail");
                }
            }
            catch (Exception er)
            {
                TempData["errorMessage"] = "خطا در ساخت پیام جدید";
                return RedirectToPage("/CRM/TicketInDetail");
            }
            
            TempData["errorMessage"] = "لطفا اطلاعات را تکمیل کنید.";
            return RedirectToPage("/CRM/TicketInDetail");
        }
        public async Task AddTicketDtlAttachment(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await UploadedFile.CopyToAsync(memoryStream);
            var newTicketAttachment = new TicketAttachment()
            {
                SortOrder = TicketAttachment.SortOrder,
                AttachDes = TicketAttachment.AttachDes,
                AttachDate = ConvertToPersianDate(gregorianToday, shamsiToday, true),
                AttachmentSubjectSN = 1,
                TicketSN = this.ticketSN,
                FileExtention = Path.GetExtension(UploadedFile.FileName),
                FileName = UploadedFile.FileName,
                FileSize = UploadedFile.Length,
                FileSave = memoryStream.ToArray(),
                

            };
            await _databaseService.AddTicketDtlAttachment(newTicketAttachment);
        }
        public async Task<IActionResult> OnPostDownloadFile(long attachmentSN)
        {
            try
            {
                var attachmentFile = await _databaseService.GetAttachmentFileAsync(attachmentSN);
                if (attachmentFile.FileSave == null)
                {
                    TempData["errorMessage"] = "خطا در دانلود";
                    return RedirectToPage();
                }
                var fileContent = attachmentFile.FileSave;
                var fileName = attachmentFile.FileName;
                var mimetype = GetMimeType(attachmentFile.FileExtention);
                Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
                //var fileExtention = attachmentFile.FileExtention.StartsWith('.') ? attachmentFile.FileExtention.Substring(1) : attachmentFile.FileExtention;                
                return File(fileContent, mimetype, fileName);
            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "خطا در دانلود فایل";
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
        private string GetMimeType(string fileExtension)
        {
            return fileExtension.ToLower() switch
            {
                "jpg" or "jpeg" => "image/jpeg",
                ".jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                ".png" => "image/png",
                "pdf" => "application/pdf",
                ".pdf" => "application/pdf",
                "txt" => "text/plain",
                ".txt" => "text/plain",
                "doc" or "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" or "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "xls" or "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" or "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ppt" or "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".ppt" or "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                _ => "application/octet-stream"
            };
        }
    }
}
