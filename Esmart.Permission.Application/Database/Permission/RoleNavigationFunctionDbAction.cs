using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.Data
{
    public class RoleNavigationFunctionDbAction
    {
        /// <summary>
        /// 删除当前角色Id对应的数据
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public static bool Del(int roleId,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            var entities = engine.Esmart_Sys_Role_Navigation_Function.Where(a => a.RoleId == roleId).ToList();
            engine.Esmart_Sys_Role_Navigation_Function.RemoveRange(entities);
            var result = engine.SaveChanges();

            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Role_Navigation_Function", OprUserId = optUserId, OptDescription = string.Format("用户：{0}{1}了角色菜单功能关系,角色ID：{2}", optUserId, "删除", roleId), Remark = JsonConvert.SerializeObject(entities) });
            return result > 0;
        }

        public static List<MenuResponse> GetMenuFunctions(List<int> roles, int appId)
        {
            if (roles == null || roles.Count == 0)
                return new List<MenuResponse>(0);

            var engine = PermissionDb.CreateEngine();

            var data = from roleNav in engine.Esmart_Sys_Role_Navigations
                       join menu in engine.Esmart_Sys_Navigations
                       on roleNav.NavigationId equals menu.NavigationId
                       where roles.Contains(roleNav.RoleId) && menu.AppId == appId
                       orderby menu.SortNo descending
                       select new MenuResponse
                       {
                           Iconurl = menu.Iconurl,
                           InClassName = menu.InClassName,
                           Id = menu.NavigationId,
                           OutClassName = menu.OutClassName,
                           ParentId = menu.ParentID,
                           Name = menu.Title,
                           Url = !string.IsNullOrEmpty(menu.Url) ? (menu.Url.IndexOf("?") == -1 ? menu.Url + "?PageId=" + menu.NavigationId : "&PageId=" + menu.NavigationId) : "#"
                       };

            return data.Distinct().ToList();
        }

        public static List<MenuResponse> GetAdminMenu(int appId)
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Sys_Navigations.Where(a => a.AppId == appId).OrderByDescending(a => a.SortNo).ToList().ConvertAll<MenuResponse>(menu =>
            {
                return new MenuResponse()
                {
                    Iconurl = menu.Iconurl,
                    InClassName = menu.InClassName,
                    Id = menu.NavigationId,
                    OutClassName = menu.OutClassName,
                    ParentId = menu.ParentID,
                    Name = menu.Title,
                    Url = !string.IsNullOrEmpty(menu.Url) ? (menu.Url.IndexOf("?") == -1 ? menu.Url + "?PageId=" + menu.NavigationId : menu.Url + "&PageId=" + menu.NavigationId) : "#"

                };
            });
        }

        public static List<FunctionModel> GetFunctionsByRoleIdsAndNavigationId(List<int> roleIds, int navigationId)
        {
            var engine = PermissionDb.CreateEngine();

            var query = from rolefun in engine.Esmart_Sys_Role_Navigation_Function
                        join p in engine.Esmart_Sys_Functions
                        on rolefun.FunctionId equals p.FunctionId
                        where roleIds.Contains(rolefun.RoleId) && rolefun.NavigationId == navigationId
                        select new FunctionModel
                        {
                            AppId = p.AppId,
                            FunctionId = p.FunctionId,
                            FunctionName = p.FunctionName,
                            Remark = p.Remark,
                            FunctionKey = p.FunctionKey,
                            CreateId = p.CreateId,
                            CreateTime = p.CreateTime,
                        };

            return query.Distinct().ToList();
        }

        /// <summary>
        /// 获取管理员的权限
        /// </summary>
        /// <param name="navigationId"></param>
        /// <returns></returns>
        public static List<FunctionModel> GetFunctionsByNavigationId(int navigationId)
        {
            var engine = PermissionDb.CreateEngine();

            var data = from rolefun in engine.Esmart_Sys_Navigation_Function
                       join p in engine.Esmart_Sys_Functions
                       on rolefun.FunctionId equals p.FunctionId
                       where rolefun.NavigationId == navigationId
                       select new FunctionModel
                       {
                           AppId = p.AppId,
                           FunctionId = p.FunctionId,
                           FunctionName = p.FunctionName,
                           Remark = p.Remark,
                           FunctionKey = p.FunctionKey,
                           CreateId = p.CreateId,
                           CreateTime = p.CreateTime,
                       };
            return data.Distinct().ToList();
        }
    }
}
