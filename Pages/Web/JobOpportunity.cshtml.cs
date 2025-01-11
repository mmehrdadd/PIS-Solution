using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.CRM;
using PIS_Web.Model.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Claims;
using TF.CommonComponent;
using TF.MessageManagement.WebReference.SMSirTicketingWS;
using PIS_Web.Services;

namespace PIS_Web.Pages.Web
{
    [Authorize]
    [Authorize(Roles = "Admin,User")]
    public class JobOpportunityModel : PageModel
    {
        public string? ErrorMessage { get; set; }
        [BindProperty]
        public JobOpportunity JobOpportunity { get; set; }
        public IEnumerable<JobOpportunity>? JobOpportunities { get; set; }
        [BindProperty]
        public int PageNumber { get; set; }
        public int? TotalPages { get; set; }
        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        [BindProperty]
        public string? SelectedSN { get; set; }

        private FormSate formSate = FormSate.View;
        private const int PageSize = 5;
        private readonly ILogger<JobOpportunityModel> _logger;
        private readonly DatabaseService _databaseService;
        public JobOpportunityModel(DatabaseService databaseService, ILogger<JobOpportunityModel> logger)
        {
            _logger = logger;
            _databaseService = databaseService; 
        }

        public async Task OnGetAsync(int PageNumber = 1)
        {
            ErrorMessage = TempData["errorMessage"] as string;
            try
            {
                JobOpportunities = await _databaseService.GetJobOpportunitiesAsync(PageNumber, PageSize);
                if (JobOpportunities.Any())
                {
                    TotalPages = (int)Math.Ceiling(JobOpportunities.FirstOrDefault().TotalCount / (double)PageSize);
                    this.PageNumber = PageNumber;
                }
                
            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "خطا در دریافت اطلاعات";
                RedirectToPage();
                
            }
            

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (formSate == FormSate.New)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (UploadedFile != null)
                        {
                            await UploadedFile.CopyToAsync(memoryStream);

                        }

                        var uploadedJO = new JobOpportunity
                        {
                            NationalID = JobOpportunity.NationalID,
                            FirstName = JobOpportunity.FirstName,
                            LastName = JobOpportunity.LastName,
                            CityDs = JobOpportunity.CityDs,
                            Mobile = JobOpportunity.Mobile,
                            Email = JobOpportunity.Email,

                            Host_Name = CCommonFunction.GetHostName(),
                            UserID_Name = User.FindFirstValue("UserSN").ToString(),
                            DefaultLanguageSN = 72
                        };
                        if (memoryStream != null)
                        {
                            uploadedJO.FileSave = memoryStream.ToArray();
                            uploadedJO.FileName = UploadedFile.FileName;
                        }
                        await _databaseService.AddJobOpportunityAsync(uploadedJO);
                    }
                    return RedirectToPage("/Web/JobOpportunity");

                }
                catch (Exception er)
                {
                    _logger.LogError(er.Message);
                    TempData["errorMessage"] = "خطا در بارگذاری اطلاعات";
                    return RedirectToPage("/Web/JobOpportunity");

                }
            }
            if (formSate == FormSate.Edit)
            {
                // Edit Logic 
                return RedirectToPage("/Web/JobOpportunity");
            }
            
              return RedirectToPage("/Web/JobOpportunity");
            
            
        }
        public IActionResult OnPostReset()
        {
            return RedirectToPage("/Web/JobOpportunity");
            
        }
        public void OnPostNew()
        {
            formSate = FormSate.New;
        }
        public void OnPostEdit()
        {
            formSate = FormSate.Edit;
        }
    }
}
