using System.Collections.Generic;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;

namespace Esmart.Permission.Web.Models.Permissions
{
    public class UserPermissionsViewModel
    {
        public UserPermissionsViewModel(int roleId, int userId)
        {
            RoleId = roleId;
            UserId = userId;
            Init();
        }

        public IEnumerable<AppShortInfo> Apps { get; private set; }

        public int RoleId { get; set; }

        public int UserId { get; set; }

        private void Init()
        {
            Apps = AppManagerDb.GetAppList().ConvertAll(a => new AppShortInfo() { AppId = a.AppID, AppName = a.AppName, Domain = a.Domain });
        }
    }
}