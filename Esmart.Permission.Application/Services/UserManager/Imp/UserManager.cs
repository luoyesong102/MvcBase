using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Esmart.Permission.Application.Constants;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Caching;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application
{
    public class UserManager : IUser
    {
        public List<AppShortInfo> GetAppListByUserId(int userId)
        {
            if (CommonAction.IsSysAdmin(userId))
            {
                return AppManagerDb.GetAppList().ConvertAll(a => new AppShortInfo() { AppId = a.AppID, AppName = a.AppName, Domain = a.Domain });
            }

            var engine = PermissionDb.CreateEngine();

            //engine.Database.Log = log => System.Diagnostics.Debug.WriteLine(log);

            var roleIds = engine.Esmart_Sys_User_Roles.Where(n => n.UserId == userId).Select(n => n.RoleId).ToArray();
            var userAppIds = ((from nav in engine.Esmart_Sys_Navigations
                              join roleNav in engine.Esmart_Sys_Role_Navigations on nav.NavigationId equals roleNav.NavigationId
                              where roleIds.Contains(roleNav.RoleId)
                              group nav by nav.AppId into appid
                              select appid.Key).Union
                              (from nav in engine.Esmart_Sys_Navigations
                                join userNav in engine.Esmart_Sys_User_Navigations on nav.NavigationId equals userNav.NavigationId
                                where userNav.UserId==userId
                                group nav by nav.AppId into appid
                                select appid.Key)).ToList();
            var apps = AppManagerDb.GetAppList().Where(n => userAppIds.Contains(n.AppID));

            return apps.Select(n => new AppShortInfo() { AppId = n.AppID, AppName = n.AppName, Domain = n.Domain }).ToList();
        }

        public bool DeleteUser(int pm_UserID, int departMentId)
        {
            DepartmentUserDbAction.DelDepartmentRelation(pm_UserID, departMentId);
            return true;
        }

        public int CreateUserWithDepartmentId(Esmart_Sys_Users user, int departMentId)
        {
            if (user.UserID == 0)
            {
                user.CreateTime = DateTime.Now;
                UserManagerDb.Add(user);
            }

            Esmart_Sys_Department_User departUser = new Esmart_Sys_Department_User
            {
                DeparentId = departMentId,
                UserId = user.UserID,
                CreateTime = DateTime.Now,
                CreateId = user.CreateId
            };

            DepartmentUserDbAction.Add(departUser);

            CommonAction.ClearCache(user.UserID);

            return user.UserID;
        }

        public bool UpdateUser(UpdateUserDto user)
        {
            UserManagerDb.Update(user);

            CommonAction.ClearCache(user.UserID);

            return true;
        }

        public bool UpdateUser2(Esmart_Sys_Users user)
        {
            UserManagerDb.Update2(user);

            CommonAction.ClearCache(user.UserID);

            return true;
        }

        public List<Esmart_Sys_Users> GetUsers(Esmart_Sys_Users condition)
        {
            return UserManagerDb.GetSysUsers(condition);
        }

        public Esmart_Sys_Users GetSingleUser(int userId)
        {
            return UserManagerDb.GetSingle(u => u.UserID == userId);
        }

        /// <summary>
        /// 根据当前用户获取权限
        /// </summary>
        /// <param name="userId">当前用户ID</param>
        /// <param name="appId">AppID</param>
        /// <param name="roleId">角色ID</param>
        /// <returns></returns>
        public RolePermissionsResponseModel GetMenuResponses(int userId, int appId, int roleId)
        {
            var ulistAll = GetMenuAndFunctionByUserId(userId, appId);
            var rlistAll = GetMenuAndFunctionByRoleId(roleId, appId);
            var responseModel = new RolePermissionsResponseModel
            {
                AllPermissions = ulistAll,
                CurrentPermissions = rlistAll,
                RoleId = roleId
            };
            return responseModel;
        }

        public void ComparisonMenu(List<MenuResponse> list, MenuResponse menu)
        {
            if (list != null)
            {
                foreach (MenuResponse menuitem in list)
                {
                    if (menu.Id != menuitem.Id)
                    {
                        ComparisonMenu(menuitem.Children, menu);
                    }
                    else
                    {
                        menu.IsCheck = true;
                        foreach (FunctionSortInfo fun in menu.Functions)
                        {
                            foreach (FunctionSortInfo f in menuitem.Functions)
                            {
                                if (fun.Id == f.Id)
                                {
                                    fun.IsCheck = true;
                                }
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        ///根据用户ID和APPId获取菜单权限列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<MenuResponse> GetMenuAndFunctionByUserId(int userId, int appId)
        {
            List<RoleResponse> roles;

            var listMenu = GetMenuByUserId(userId, appId, out roles);

            var menusAndFuns = CommonAction.IsSysAdmin(userId)
                ? Data.MenuManager.GetAllMensFunctions()
                : Data.MenuManager.GetAllMensFunctions(roles.Select(n => n.RoleId).ToList());

            foreach (var menu in listMenu)
            {
                SetFunctions(menu, menusAndFuns);
            }
            return listMenu;
        }

        /// <summary>
        ///根据角色ID和APPId获取菜单权限列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public List<MenuResponse> GetMenuAndFunctionByRoleId(int roleId, int appId)
        {
            List<MenuResponse> listMenu = GetMenusByRoleId(roleId, appId);

            var menusAndFuns = Data.MenuManager.GetAllMensFunctions(new List<int>() { roleId });

            foreach (var menu in listMenu)
            {
                SetFunctions(menu, menusAndFuns);
            }
            return listMenu;
        }

        public MenuResponses GetMenuByUserId(int userId, int appId)
        {
            var key = "GetMenuByUserId" + userId + appId;

            var data = CacheManager.CreateCache().Get<MenuResponses>(key);

            if (data != null && data.Count > 0)
            {
                return data;
            }

            List<RoleResponse> roles;

            data = new MenuResponses
            {
                Menus = GetMenuByUserId(userId, appId, out roles),
                HomeUrl = GetHomeUrl(roles.Select(n => n.RoleName))
            };

            CacheManager.CreateCache().Set(key, data);

            return data;
        }

        private static string GetHomeUrl(IEnumerable<string> roleNames)
        {
            var set = new HashSet<string>(roleNames);
            return ConfigurationManager.AppSettings["Default_HomeUrl"];
        }

        public List<MenuResponse> GetMenuByUserId(int userId, int appId, out List<RoleResponse> roles)
        {
            roles = UserManagerDb.GetRolesByUserId(userId);

            List<MenuResponse> menus = null;
            if(CommonAction.IsSysAdmin(userId))
            {
                menus = RoleNavigationFunctionDbAction.GetAdminMenu(appId);
            }
            else
            {
                menus = new List<MenuResponse>();
                menus.AddRange(RoleNavigationFunctionDbAction.GetMenuFunctions(roles.Select(n => n.RoleId).ToList(), appId));
                menus.AddRange(UserNavigationFunctionDbAction.GetMenuFunctions(new List<int> { userId }, appId));
                menus= menus.Where((i,j) =>menus.FindIndex(f=>f.Id==i.Id)==j).ToList();
            }
            //var menus = CommonAction.IsSysAdmin(userId)
            //    ? RoleNavigationFunctionDbAction.GetAdminMenu(appId)
            //    : RoleNavigationFunctionDbAction.GetMenuFunctions(roles.Select(n => n.RoleId).ToList(), appId);

            var topMenus = new List<MenuResponse>();

            foreach (var fun in menus)
            {
                GetTopMenu(fun, menus, topMenus);
            }

            foreach (var menu in topMenus)
            {
                SetChildren(menus, menu);
            }

            return topMenus;
        }

        public List<MenuResponse> GetMenusByRoleId(int roleId, int appId)
        {
            var menus = RoleNavigationFunctionDbAction.GetMenuFunctions(new List<int>() { roleId }, appId);

            List<MenuResponse> topMenus = new List<MenuResponse>();

            foreach (var fun in menus)
            {
                GetTopMenu(fun, menus, topMenus);
            }
            foreach (var menu in topMenus)
            {
                SetChildren(menus, menu);
            }
            return topMenus;
        }

        private static void GetTopMenu(MenuResponse parent, List<MenuResponse> menus, List<MenuResponse> topMenus)
        {
            var top = menus.Find(a => a.Id == parent.ParentId);
            if (top != null)
            {
                GetTopMenu(top, menus, topMenus);
            }
            else
            {
                if (topMenus.Find(a => a.Id == parent.Id) == null)
                {
                    topMenus.Add(parent);
                }
            }
        }

        private static void SetChildren(List<MenuResponse> menus, MenuResponse parent)
        {
            var chidren = menus.FindAll(a => a.ParentId == parent.Id);

            if (chidren.Count == 0)
            {
                return;
            }

            parent.Children = chidren;

            foreach (var child in parent.Children)
            {
                SetChildren(menus, child);
            }

        }

        private static void SetFunctions(MenuResponse menu, List<MenuFunctionModel> functions)
        {
            var children = functions.FindAll(a => a.NavigationId == menu.Id);

            if (children.Count > 0)
            {
                menu.Functions = children.ConvertAll(a => new FunctionSortInfo() { Id = a.FunctionId ?? 0, Key = a.FunctionKey, Name = a.FunctionName });
            }

            if (menu.Children == null || menu.Children.Count <= 0) return;

            foreach (var chidMenu in menu.Children)
            {
                SetFunctions(chidMenu, functions);
            }
        }

        public List<FunctionModel> GetFunctionByUserIdAndMenuId(int userId, int menuId)
        {
            string cachekey = userId + "-" + menuId;

            var list = CacheManager.CreateCache().Get<List<FunctionModel>>(cachekey);

            if (list != null && list.Count > 0)
            {
                return list;
            }

            list = new List<FunctionModel>(20);

            if (CommonAction.IsSysAdmin(userId))//如果是管理员 获取菜单所有的功能
            {
                var builtin = BuiltinRoles.All.Select(n => new FunctionModel { FunctionKey = n, FunctionName = n });
                list.Add(new FunctionModel { FunctionKey = "$ADMIN", FunctionName = "$ADMIN" });
                list.AddRange(builtin);
                list.AddRange(RoleNavigationFunctionDbAction.GetFunctionsByNavigationId(menuId));
            }
            else
            {
                var roles = UserRolesDbAction.GetUserRolses(userId);

                roles.ForEach(n =>
                {
                    if (BuiltinRoles.All.Contains(n.RoleName))
                    {
                        list.Add(new FunctionModel { FunctionKey = n.RoleName, FunctionName = n.RoleName });
                    }
                });

                //角色分配的功能
                var roleIds = roles.Select(p => p.RoleId).ToList();
                ICollection<FunctionModel> funcList = RoleNavigationFunctionDbAction.GetFunctionsByRoleIdsAndNavigationId(roleIds, menuId);
                ICollection<FunctionModel> userFuncList = UserNavigationFunctionDbAction.GetFunctionsByUserIdsAndNavigationId(new List<int> { userId }, menuId);
                
                list.AddRange(funcList);
                list.AddRange(userFuncList.Except(funcList).ToList());
            }

            CacheManager.CreateCache().Add(cachekey, list);

            return list;
        }

        /// <summary>
        /// 获取当前部门以外的用户列表 by wangligui
        /// </summary>
        public List<DepartmentUserResponse> GetUserOutDepartment(int departmentId, int loginUserId)
        {
            var isSysAdmin = CommonAction.IsSysAdmin(loginUserId);
            return DepartmentUserDbAction.GetUserOutDepartment(departmentId, loginUserId, isSysAdmin);
        }

        public bool ResetUserPwd(int userId)
        {
            return UserManagerDb.ResetPwd(userId);
        }

        /// <summary>
        /// 设置用户的离职状态  1:离职  0：未离职   by wangligui
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateLeaveStatus(int userId)
        {
            return UserManagerDb.UpdateLeaveStatus(userId);
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        public bool ModifyPassword(string userAccount, string password, string newPassword)
        {
            if (newPassword.Length < 6)
            {
                throw new TpoBaseException("密码长度最低需要6位，需同时包含大写字母、小写字母和数字");
            }

            var flag = 0;

            foreach (var c in newPassword)
            {
                if (c >= '0' && c <= '9')
                    flag |= 1;
                if (c >= 'a' && c <= 'z')
                    flag |= 2;
                if (c >= 'A' && c <= 'Z')
                    flag |= 4;
            }

            if (flag != (1 | 2 | 4))
            {
                throw new TpoBaseException("密码需同时包含大写字母、小写字母和数字");
            }

            return UserManagerDb.ModifyPassword(userAccount, password, newPassword);
        }

        public bool ChangeDepartmentOfUser(List<int> userIds, int newDepartmentId, int createId)
        {
            DepartmentUserDbAction.ChangeDepartmentOfUser(userIds, newDepartmentId, createId);
            return true;
        }

        public List<string> GetUserRole(int userId)
        {
            var cacheKey = CacheKey.Authorize_UserRole + userId;

            var data = CacheManager.CreateCache().Get<List<string>>(cacheKey);
            if (data != null) return data;

            data = UserManagerDb.GetRolesByUser(userId);

            CacheManager.CreateCache().Set(cacheKey, data);

            return data;
        }

        ///////////////////////////////////////
        public UserPermissionsResponseModel GetUserMenuResponses(int userId, int appId, int selUserId)
        {
            var ulistAll = GetMenuAndFunctionByUserId(userId, appId);
            var rlistAll =GetMenuAndFunctionByChkUserId(selUserId, appId);
            UserManagerDb.GetRolesByUserId(selUserId).ForEach(o => 
            {
               rlistAll.AddRange( GetMenuAndFunctionByRoleId(o.RoleId,appId));
            });
            rlistAll=rlistAll.Distinct().ToList();
            //GetMenuAndFunctionByRoleId(selUserId,appId);
            var responseModel = new UserPermissionsResponseModel
            {
                AllPermissions = ulistAll,
                CurrentPermissions = rlistAll,
                UserId = userId
            };
            return responseModel;
        }

        public List<MenuResponse> GetMenuAndFunctionByChkUserId(int userId, int appId)
        {
            List<MenuResponse> listMenu = GetMenusByChkUserId(userId, appId);

            var menusAndFuns = Data.MenuManager.GetUserAddAllMensFunctions(new List<int>() { userId });

            foreach (var menu in listMenu)
            {
                SetFunctions(menu, menusAndFuns);
            }
            return listMenu;
        }

        public List<MenuResponse> GetMenusByChkUserId(int userId, int appId)
        {
            var menus = UserNavigationFunctionDbAction.GetMenuFunctions(new List<int>() { userId }, appId);

            List<MenuResponse> topMenus = new List<MenuResponse>();

            foreach (var fun in menus)
            {
                GetTopMenu(fun, menus, topMenus);
            }
            foreach (var menu in topMenus)
            {
                SetChildren(menus, menu);
            }
            return topMenus;
        }

        public List<UsersView> GetUsersByDepartList(int userId,string groupName,int departId=0)
        {
            List<UsersView> lstResult = null;
            try
            {
                SoaDataPage<UserSearchModel> filter = new SoaDataPage<UserSearchModel>(); ;
                CommonFunction.InitializeSoaDataPage(filter);
                filter.Where.DeapartmentId = departId;
                filter.Where.UserId = userId;
                filter.PageIndex = 1;
                filter.PageSize = 100000000;
                var result = new Esmart.Permission.Application.DeparentManager.DepartmentManager().GetUsersByDepartList(filter);
                var resultEnd= result.Body.Where(f => f.RoleNames.Split(',').Contains(groupName));
                if(resultEnd!=null)
                {
                    lstResult = resultEnd.ToList();
                }
            }catch(Exception ex)
            {
                throw ex;
            }
            return lstResult;
        }

        public List<UserLiteDto> GetGroupUsersByLoginUserID(int userId)
        {
            List<UserLiteDto> lstResult = null;
            try
            {
                SoaDataPage<UserSearchModel> filter = new SoaDataPage<UserSearchModel>(); ;
                CommonFunction.InitializeSoaDataPage(filter);
                filter.Where.UserId = userId;
                filter.PageIndex = 1;
                filter.PageSize = 100000000;
                var result = new Esmart.Permission.Application.DeparentManager.DepartmentManager().GetUsersByDepartList(filter);
                if (result != null&&!CommonAction.IsSysAdmin(userId))
                {
                    List<UsersView> lstDepart = DepartmentUserDbAction.GetGroupsByUserId(userId);
                    lstResult = result.Body.Where(w => lstDepart.Find(f => f.UserID == w.UserID) != null).ToList().ConvertAll(c => new UserLiteDto { UserID=c.UserID,TrueName=c.TrueName,WorkNo=c.WorkNo,Email=c.Email,Ename=c.Ename,Sex=c.Sex});
                }else
                {
                    lstResult = result.Body.ToList().ConvertAll(c => new UserLiteDto { UserID = c.UserID, TrueName = c.TrueName, WorkNo = c.WorkNo, Email = c.Email, Ename = c.Ename, Sex = c.Sex });
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstResult;
        }
    }
}
