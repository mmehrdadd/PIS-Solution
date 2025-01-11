namespace PIS_Web.Model.Web
{
    public class JobOpportunity
    {
        public decimal JobOpportunitySN { get; set; } 
        public string NationalID { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string CityDs { get; set; } 
        public string Mobile { get; set; } 
        public string Email { get; set; } 
        public string? FileName { get; set; } 
        public byte[]? FileSave { get; set; } 
        public long DefaultLanguageSN { get; set; }
        public string? UserID_Name { get; set; }
        public string? Host_Name { get; set; }
        public int TotalCount { get; set; }
    }
}
