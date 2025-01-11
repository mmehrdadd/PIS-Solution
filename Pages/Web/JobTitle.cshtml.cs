using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PIS_Web.Model.Web;

namespace PIS_Web.Pages.Web
{
    public class JobTitleModel : PageModel
    {
        private readonly ILogger<JobTitleModel> _logger;
        private readonly DatabaseService _databaseService;
        private const int PageSize = 5;
        public int TotalPages;
        [BindProperty]
        public int PageNumber {  get; set; }   

        public JobTitleModel(DatabaseService databaseService,ILogger<JobTitleModel> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        public JobTitle JobTitle { get; set; }
        public IEnumerable<JobTitle>? JobTitles { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGet(int PageNumber = 1)
        {
            ErrorMessage = TempData["errorMessaege"] as string;

            try
            {
                JobTitles = await _databaseService.GetJobTitleAsync(PageNumber,PageSize);
                if (JobTitles.Any())
                {
                    TotalPages = (int)Math.Ceiling(JobTitles.FirstOrDefault().TotalCount / (double)PageSize);
                    this.PageNumber = PageNumber;
                }
            }
            catch (Exception er)
            {
                _logger.LogError(er.Message);
                TempData["errorMessage"] = "خطا در بارگذاری اطالاعات";
                RedirectToPage("/Web/JobTitle");
            }
        }
    }
}
