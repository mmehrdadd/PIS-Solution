namespace PIS_Web.Services
{
    public interface ISecurityManager
    {
        bool HasAccess(string pageName, string action, string userSN);
    }

}
