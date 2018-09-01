using System.Collections.Generic;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.AppManager
{
    [ServerAction(ServerType = typeof(AppManager))]
    public interface IApp
    {
        List<AppShortInfo> GetAppList();
    }
}
