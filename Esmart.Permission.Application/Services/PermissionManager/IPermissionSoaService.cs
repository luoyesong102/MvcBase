using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.PermissionManager
{
     [ServerAction(ServerType = typeof(PermissionSoaService))]
    public interface IPermissionSoaService
     {
         /// <summary>
         /// 给角色分配权限
         /// </summary>
         /// <param name="request">PermissionManagerRequest</param>
         /// <returns>true|false</returns>
         bool AssignPermissionRole(RolePermissionsRequestModel request);
         /// <summary>
         /// 给用户分配权限
         /// </summary>
         /// <param name="request"></param>
         /// <returns></returns>
         bool AssignPermissionUser(UserPermissionsRequestModel request);
     }
}
