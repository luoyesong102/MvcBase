using System;
using System.Collections.Generic;
using Esmart.Permission.Application.RoleManager;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web
{
    public class RoleService
    {
        readonly IRoleSoaService _roleService;

        public RoleService()
        {
            _roleService = new RoleSoaService();
        }

        public SoaDataPageResponse<RoleModel> GetDepartmentRoles(SoaDataPage<RoleGridFilterViewModel> fiter)
        {
            try
            {
                return _roleService.GetRoleList(fiter);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public RoleModel GetRole(int roleId)
        {
            try
            {
                return _roleService.GetRoleRequest(roleId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool SaveRole(RoleModel role)
        {
            try
            {
                return _roleService.AddOrUpdate(role);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool DeleteRole(int roleId)
        {
            try
            {
                var roleModel = new RoleModel()
                {
                    Id = roleId
                };
                return _roleService.Del(roleModel);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public List<RoleResponse> GetAssignRoles(int loginUserId, int depId, int userId)
        {
            try
            {
                return _roleService.GetAssignRoles(loginUserId, depId, userId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public List<DepartmentUserResponse2> GetUsersOfRole(int roleId)
        {
            try
            {
                return _roleService.GetUsersOfRole(roleId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public int UpdateUserRole(int userId, int creatId, List<int> listRole)
        {
            try
            {
                return _roleService.UpdateUserRole(userId, creatId, listRole);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public int UpdateDepartmentRole(int depId, int creatId, List<int> listRole)
        {
            try
            {
                return _roleService.UpdateDepartmentRole(depId, creatId, listRole);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public int RemoveUserRole(int userId, int roleId)
        {
            try
            {
                return _roleService.RemoveUserRole(userId, roleId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }
    }
}
