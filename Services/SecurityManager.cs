using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using PIS_Web.Model.Common;
using System.Text.Json.Serialization;

namespace PIS_Web.Services
{
    public class SecurityManager : ISecurityManager
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDataProtector _dataProtector;

        public SecurityManager(IHttpContextAccessor contextAccessor, IDataProtector dataProtector)
        {
            _contextAccessor = contextAccessor;
            _dataProtector = dataProtector;
        }

        public bool HasAccess(string pageName, string action, string userSN)
        {
            var cookie = _contextAccessor.HttpContext.Request.Cookies["SM"];
            if (cookie == null)
            {
                return false;
            }
            try
            {
                var decryptedData = _dataProtector.Unprotect(cookie);
                var userAccess = JsonConvert.DeserializeObject<UserAccess>(decryptedData);

                var pageAccess = userAccess.AccessList.FirstOrDefault(p => p.PageName == pageName);
                if (pageAccess == null)
                {
                    return false;
                }
                return action switch
                {
                    "View" => pageAccess.CanView,
                    "Edit" => pageAccess.CanEdit,
                    "Delete" => pageAccess.CanDelete,
                    "Create" => pageAccess.CanCreate
                };
            }
            catch 
            {
                return false;               
            }
            
        }
    }
}
