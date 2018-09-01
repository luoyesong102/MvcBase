using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.Data
{
    public class PermissionDbAction
    {
        /// <summary>
        /// 给角色添加菜单和功能
        /// </summary>
        /// <returns></returns>
        public static bool AssignPermissionRole(int roleId, int appId, List<Esmart_Sys_Role_Navigations> roleNavigationses, List<Esmart_Sys_Role_Navigation_Function> roleNavigationFunctions)
        {
            var engine = PermissionDb.CreateEngine();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //获取要删除的Esmart_Sys_Role_Navigations集合
            var navigations = (from a in engine.Esmart_Sys_Role_Navigations.ToList()
                               join b in engine.Esmart_Sys_Navigations.ToList()
                                   on a.NavigationId equals b.NavigationId
                               where a.RoleId == roleId && b.AppId == appId
                               select a).ToList();

            //获取要删除的Esmart_Sys_Role_Navigation_Function集合
            var functions = (from a in engine.Esmart_Sys_Role_Navigation_Function.ToList()
                             join b in engine.Esmart_Sys_Functions.Where(a => a.AppId == appId).ToList()
                                 on a.FunctionId equals b.FunctionId
                             where a.RoleId == roleId
                             select a).ToList();

            //删除Esmart_Sys_Role_Navigations
            engine.Esmart_Sys_Role_Navigations.RemoveRange(navigations);

            //删除Esmart_Sys_Role_Navigation_Function
            engine.Esmart_Sys_Role_Navigation_Function.RemoveRange(functions);

            if (roleNavigationses.Count > 0)
            {
                //批量添加Esmart_Sys_Role_Navigations
                engine.Esmart_Sys_Role_Navigations.AddRange(roleNavigationses);

                //批量添加Esmart_Sys_Role_Navigation_Function
                engine.Esmart_Sys_Role_Navigation_Function.AddRange(roleNavigationFunctions);
            }
            else if (roleNavigationFunctions.Count > 0)
            {
                //批量添加Esmart_Sys_Role_Navigation_Function
                engine.Esmart_Sys_Role_Navigation_Function.AddRange(roleNavigationFunctions);
            }

            //一起提交到数据库  事务
            engine.SaveChanges();
            int createId = roleNavigationses.First().CreateId;
            sb.Append(JsonConvert.SerializeObject(navigations)).Append(";").Append(JsonConvert.SerializeObject(functions)).Append(";").Append(JsonConvert.SerializeObject(roleNavigationses)).Append(";").Append(JsonConvert.SerializeObject(roleNavigationFunctions));
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = createId, CreateTime = System.DateTime.Now, EventType = "add&delete", OprTbName = "Esmart_Sys_Role_Navigations,Esmart_Sys_Role_Navigation_Function", OprUserId = createId, OptDescription = string.Format("用户：{0}修改了角色菜单关系,其中删除角色菜单关系表ID：{1},删除角色菜单功能关系表ID：{2},添加角色菜单关系表ID：{3},添加角色菜单功能关系表ID：{4}", createId, string.Join(",", navigations.Select(s => s.Id)), string.Join(",", functions.Select(s => s.Id)), string.Join(",", roleNavigationses.Select(s => s.Id)), roleNavigationFunctions.Select(s => s.Id)), Remark =sb.ToString() });
            CommonAction.ClearCache();
            sb.Length = 0;
            return true;
        }

        /// <summary>
        /// 给用户添加菜单和功能
        /// </summary>
        /// <returns></returns>
        public static bool AssignPermissionUser(int userId, int appId, List<Esmart_Sys_User_Navigations> userNavigationses, List<Esmart_Sys_User_Navigation_Function> userNavigationFunctions)
        {
            var engine = PermissionDb.CreateEngine();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //获取要删除的Esmart_Sys_Role_Navigations集合
            var navigations = (from a in engine.Esmart_Sys_User_Navigations.ToList()
                               join b in engine.Esmart_Sys_Navigations.ToList()
                                   on a.NavigationId equals b.NavigationId
                               where a.UserId == userId && b.AppId == appId
                               select a).ToList();

            //获取要删除的Esmart_Sys_User_Navigation_Function集合
            var functions = (from a in engine.Esmart_Sys_User_Navigation_Function.ToList()
                             join b in engine.Esmart_Sys_Functions.Where(a => a.AppId == appId).ToList()
                                 on a.FunctionId equals b.FunctionId
                             where a.UserId == userId
                             select a).ToList();

            //删除Esmart_Sys_Role_Navigations
            engine.Esmart_Sys_User_Navigations.RemoveRange(navigations);

            //删除Esmart_Sys_Role_Navigation_Function
            engine.Esmart_Sys_User_Navigation_Function.RemoveRange(functions);

            if (userNavigationses.Count > 0)
            {
                //批量添加Esmart_Sys_Role_Navigations
                engine.Esmart_Sys_User_Navigations.AddRange(userNavigationses);

                //批量添加Esmart_Sys_Role_Navigation_Function
                engine.Esmart_Sys_User_Navigation_Function.AddRange(userNavigationFunctions);
            }
            else if (userNavigationFunctions.Count > 0)
            {
                //批量添加Esmart_Sys_Role_Navigation_Function
                engine.Esmart_Sys_User_Navigation_Function.AddRange(userNavigationFunctions);
            }

            //一起提交到数据库  事务
            engine.SaveChanges();
            int createId = userNavigationses.First().CreateId;
            sb.Append(JsonConvert.SerializeObject(navigations)).Append(";").Append(JsonConvert.SerializeObject(functions)).Append(";").Append(JsonConvert.SerializeObject(userNavigationses)).Append(";").Append(JsonConvert.SerializeObject(userNavigationFunctions));
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = createId, CreateTime = System.DateTime.Now, EventType = "add&delete", OprTbName = "Esmart_Sys_User_Navigations,Esmart_Sys_User_Navigation_Function", OprUserId = createId, OptDescription = string.Format("用户：{0}修改了用户菜单关系,其中删除用户菜单关系表ID：{1},删除用户菜单功能关系表ID：{2},添加用户菜单关系表ID：{3},添加用户菜单功能关系表ID：{4}", createId, string.Join(",", navigations.Select(s => s.Id)), string.Join(",", functions.Select(s => s.Id)), string.Join(",", userNavigationses.Select(s => s.Id)), userNavigationFunctions.Select(s => s.Id)), Remark = sb.ToString() });
            CommonAction.ClearCache();
            return true;
        }
    }
}
