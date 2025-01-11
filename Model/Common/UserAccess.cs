namespace PIS_Web.Model.Common
{
    public class UserAccess
    {
        public string UserId { get; set; }
        public List<PageAccess> AccessList { get; set; }
    }

    public class PageAccess
    {
        public string PageName { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanCreate { get; set; }
    }
}
