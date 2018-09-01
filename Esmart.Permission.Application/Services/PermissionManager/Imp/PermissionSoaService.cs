using System;
using System.Collections.Generic;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.PermissionManager
{
    public class PermissionSoaService : IPermissionSoaService
    {
        /// <summary>
        /// 给角色分配权限
        /// </summary>
        /// <param name="request">PermissionManagerRequest</param>
        /// <returns>true|false</returns>
        public bool AssignPermissionRole(RolePermissionsRequestModel request)
        {
            //添加菜单权限和功能
            var roleNavigationses = new List<Esmart_Sys_Role_Navigations>();
            var roleNavigationFunctions = new List<Esmart_Sys_Role_Navigation_Function>();
            ConvertToDbModel(request, ref roleNavigationses, ref roleNavigationFunctions);
            return PermissionDbAction.AssignPermissionRole(request.RoleId, request.AppId, roleNavigationses,roleNavigationFunctions);
        }

        private void ConvertToDbModel(RolePermissionsRequestModel request,
            ref List<Esmart_Sys_Role_Navigations> roleNavigationses,
            ref List<Esmart_Sys_Role_Navigation_Function> roleNavigationFunctions)
        {

            if (request.NavigationsCollection != null)
            {
                foreach (var i in request.NavigationsCollection)
                {
                    roleNavigationses.Add(new Esmart_Sys_Role_Navigations()
                    {
                        RoleId = request.RoleId,
                        CreateId = request.CreatorId ?? 0,
                        CreateTime = request.CreateTime ?? DateTime.Now,
                        NavigationId = i
                    });
                }
            }

            if (request.FunctionsCollection == null) return;
            
            foreach (var i in request.FunctionsCollection)
            {
                roleNavigationFunctions.Add(new Esmart_Sys_Role_Navigation_Function()
                {
                    RoleId = request.RoleId,
                    FunctionId = i.Key,
                    NavigationId = i.Value,
                    CreateTime = request.CreateTime ?? DateTime.Now,
                    CreateId = request.CreatorId ?? 0,

                });
            }
        }

        /// <summary>
        /// 给用户分配权限
        /// </summary>
        /// <param name="request">PermissionManagerRequest</param>
        /// <returns>true|false</returns>
        public bool AssignPermissionUser(UserPermissionsRequestModel request)
        {
            //添加菜单权限和功能
            var userNavigationses = new List<Esmart_Sys_User_Navigations>();
            var userNavigationFunctions = new List<Esmart_Sys_User_Navigation_Function>();
            ConvertToUserDbModel(request, ref userNavigationses, ref userNavigationFunctions);
            return PermissionDbAction.AssignPermissionUser(request.UserId, request.AppId, userNavigationses, userNavigationFunctions);
        }

        private void ConvertToUserDbModel(UserPermissionsRequestModel request,
            ref List<Esmart_Sys_User_Navigations> userNavigationses,
            ref List<Esmart_Sys_User_Navigation_Function> userNavigationFunctions)
        {

            if (request.NavigationsCollection != null)
            {
                foreach (var i in request.NavigationsCollection)
                {
                    userNavigationses.Add(new Esmart_Sys_User_Navigations()
                    {
                        UserId = request.UserId,
                        CreateId = request.CreatorId ?? 0,
                        CreateTime = request.CreateTime ?? DateTime.Now,
                        NavigationId = i
                    });
                }
            }

            if (request.FunctionsCollection == null) return;

            foreach (var i in request.FunctionsCollection)
            {
                userNavigationFunctions.Add(new Esmart_Sys_User_Navigation_Function()
                {
                    UserId = request.UserId,
                    FunctionId = i.Key,
                    NavigationId = i.Value,
                    CreateTime = request.CreateTime ?? DateTime.Now,
                    CreateId = request.CreatorId ?? 0,

                });
            }
        }
    }
}
