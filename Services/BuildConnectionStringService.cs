using IniParser;
using IniParser.Model;
using IniParser.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PIS_Web.Services
{
    public class BuildConnectionStringService
    {
        private readonly string IniFilePath = Path.Combine(AppContext.BaseDirectory, "StartInfo.ini");
        public string IniPath { get { return IniFilePath; }}
        TF.SecurityManager.SecurityManager sm = new TF.SecurityManager.SecurityManager();
        public bool ConnectedToDataBase { get; set; }

        public string GetConnectionStringService(string section)
        {
            if (ValidateIniPath())
            {
                var parser = new IniDataParser { Configuration = { AllowDuplicateSections = true, AllowDuplicateKeys = true } };
                var fileParser = new FileIniDataParser(parser);
                IniData data = fileParser.ReadFile(IniPath);

                if (data.Sections.ContainsSection(section))
                {
                    var sectionData = data[section];
                    string serverIP = TF.CommonComponent.CCommonFunction.DecodeString(sectionData["ServerIP"]);
                    string serverName = TF.CommonComponent.CCommonFunction.DecodeString(sectionData["ServerName"]);
                    string userSN = TF.CommonComponent.CCommonFunction.DecodeString(sectionData["UserSN"]);
                    string password = TF.CommonComponent.CCommonFunction.DecodeString(sectionData["Password"]);
                    string databaseName = sectionData["Database"];
                    var connectionString = $"Data Source={serverName};Initial Catalog={databaseName};Persist Security Info=True;User Id={userSN};Password={password};Trust Server Certificate=True;";
                    return connectionString;
                    
                }
                return "";
            }
            return "";
        }

        private bool ValidateIniPath()
        {
            if (!File.Exists(IniFilePath))
            {
              ConnectedToDataBase = false;
              return false;
            }
            else 
            {
                ConnectedToDataBase = true;
                return true;

            }
        }
    }
}
