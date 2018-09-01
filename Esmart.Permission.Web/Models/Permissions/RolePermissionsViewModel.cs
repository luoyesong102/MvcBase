using System.Collections.Generic;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;

namespace Esmart.Permission.Web.Models.Permissions
{
    public class RolePermissionsViewModel
    {
        public RolePermissionsViewModel(int roleId, int userId)
        {
            RoleId = roleId;
            Init();
        }

        public IEnumerable<AppShortInfo> Apps { get; private set; }

        public int RoleId { get; set; }

        private void Init()
        {
            Apps = AppManagerDb.GetAppList().ConvertAll(a => new AppShortInfo() { AppId = a.AppID, AppName = a.AppName, Domain = a.Domain });
        }
    }
}