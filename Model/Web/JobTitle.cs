namespace PIS_Web.Model.Web
{
    public class JobTitle
    {
        public decimal JobTitleSN { get; set; }
        public decimal? FatherJobTitleSN { get; set; }
        public int? JobTitleNo { get; set; }
        public string JobTitleDs { get; set; }
        public decimal JobTitleTypeSN { get; set; }
        public string? Description { get; set; }
        public int DefaultLanguageSN { get; set; }
        public decimal SortOrder { get; set; }
        public string UnqStr { get; set; }
        public string? UserID_Name { get; set; }
        public string? Host_Name { get; set; }
        public int OldID { get; set; }
        public bool IsShowInWeb { get; set; }
        public int TotalCount { get;set; }
        public string GeneralStatusDtlDs { get; set; }
    }
}
