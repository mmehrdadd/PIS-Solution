using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PIS_Web.Model.CRM
{
    public class User
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string UserSN { get; set; }
        public bool IsStaffMember { get; set; }
        public bool IsExpert { get; set; }
        public bool IsAdmin { get; set; }
    }
}
