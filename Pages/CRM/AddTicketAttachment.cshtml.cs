using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.CRM;
using System.Globalization;
using System.Net.Mail;

namespace PIS_Web.Pages.CRM
{
    [Authorize]
    [Authorize(Roles = "Admin,User")]
    public class AddTicketAttachmentModel : PageModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<AddTicketAttachmentModel> _logger;
        public string? errorMessage { get; set; }
        public TicketView Ticket { get; set; }
        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        public IEnumerable<TicketAttachment> TicketAttachments { get; set; }
        public IEnumerable<AttachmentSubject> AttachmentSubjects { get; set; }
        [BindProperty]
        public TicketAttachment TicketAttachment { get; set; }
        [BindProperty]
        public string AttachDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal TicketSN { get; set; }

        public DateTime gregorianToday { get; set; } = DateTime.Today;
        PersianCalendar shamsiToday { get; set; } = new PersianCalendar();

        public AddTicketAttachmentModel(DatabaseService databaseService, ILogger<AddTicketAttachmentModel> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }
        public async Task OnGetAsync(decimal TicketSN)
        {
            errorMessage = TempData["errorMessage"] as string;
            try
            {
                Ticket = await _databaseService.GetTicketBySNAsync(TicketSN);
                TicketAttachments = await _databaseService.GetTicketAttachmentsAsync(TicketSN);

                AttachmentSubjects = await _databaseService.GetAttachmentSubjectsAsync();
                AttachDate = ConvertToPersianDate(gregorianToday, shamsiToday, false);

            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "خطا در دریافت اطلاعات";
                RedirectToPage();

            }
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
        public async Task<IActionResult> OnPostAsync()
        {
            if (TicketAttachment.AttachmentSubjectSN == 0)
            {
                TempData["errorMessage"] = "موضوع پیوست انتخاب نشده است.";
            }
            try
            {
                if (UploadedFile != null)
                {
                    using var memoryStream = new MemoryStream();
                    await UploadedFile.CopyToAsync(memoryStream);
                    var newTicketAttachment = new TicketAttachment()
                    {
                        SortOrder = TicketAttachment.SortOrder,
                        AttachDes = TicketAttachment.AttachDes,
                        AttachDate = ConvertToPersianDate(gregorianToday, shamsiToday, true),
                        AttachmentSubjectSN = TicketAttachment.AttachmentSubjectSN,
                        TicketSN = TicketSN,
                        FileExtention = Path.GetExtension(UploadedFile.FileName),
                        FileName = UploadedFile.FileName,
                        FileSize = UploadedFile.Length,
                        FileSave = memoryStream.ToArray(),

                    };
                    await _databaseService.SaveAttachmentForTicketAsync(newTicketAttachment);
                }
                else
                {
                    TempData["errorMessage"] = "پیوست انتخاب نشده است.";
                    return RedirectToPage();
                }
                return RedirectToPage("/CRM/AddTicketAttachment", new { TicketSN });
            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "خطا در بارگذاری پیوست";
                return RedirectToPage();
            }
        }
        public async Task<IActionResult> OnPostDelete(int AttachmentSN)
        {
            try
            {
                await _databaseService.DeleteTicketAttachmentAsync(AttachmentSN);
                return RedirectToPage("/CRM/AddTicketAttachment", new { TicketSN });
            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "خطا در حذف پیوست";
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
            return RedirectToPage("/CRM/TicketList");
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
