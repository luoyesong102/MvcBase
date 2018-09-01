using System.Collections.Generic;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;

namespace Esmart.Permission.Application.AppManager
{
    public class AppManager : IApp
    {
        public List<AppShortInfo> GetAppList()
        {
            return AppManagerDb.GetAppList().ConvertAll(a => new AppShortInfo() { AppId = a.AppID, AppName = a.AppName });
        }
    }
}
