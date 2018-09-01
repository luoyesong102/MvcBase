using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class RolePermissionsResponseModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色拥有的权限集合
        /// </summary>
        public ICollection<MenuResponse> CurrentPermissions { get; set; }

        /// <summary>
        /// 当前登录用户的最大权限集合（由当前登录用户决定）
        /// </summary>
        public ICollection<MenuResponse> AllPermissions { get; set; }
    }
}
