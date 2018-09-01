using System;
using Esmart.Permission.Application.PermissionManager;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web
{
    public class PermissionService
    {
        IPermissionSoaService _permissionService;

        public PermissionService()
        {
            _permissionService = new PermissionSoaService();
        }

        public bool AssignRolePermission(RolePermissionsRequestModel request)
        {
            try
            {
                return _permissionService.AssignPermissionRole(request);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool AssignUserPermission(UserPermissionsRequestModel request)
        {
            try
            {
                return _permissionService.AssignPermissionUser(request);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }
    }
}
