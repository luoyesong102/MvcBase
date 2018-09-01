using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Newtonsoft.Json;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Data
{
    public class MenuManager
    {
        private const string logFormat="用户：{0}{1}了菜单,ID：{2}";
        public static List<Esmart_Sys_Navigations> GetAllMenus(int appId)
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Sys_Navigations.Where(a => a.AppId == appId).OrderByDescending(a => a.SortNo).ToList();
        }

        public static List<MenuFunctionModel> GetAllMensFunctions()
        {
            var engine = PermissionDb.CreateEngine();
            var data = from fun in engine.Esmart_Sys_Functions
                       join menu in engine.Esmart_Sys_Navigation_Function
                           on fun.FunctionId equals menu.FunctionId
                       select new MenuFunctionModel()
                       {
                           FunctionId = fun.FunctionId,
                           FunctionKey = fun.FunctionKey,
                           FunctionName = fun.FunctionName,
                           NavigationId = menu.NavigationId
                       };


            return data.ToList();
        }

        public static List<MenuFunctionModel> GetAllMensFunctions(List<int> roleId)
        {
            var engine = PermissionDb.CreateEngine();
            var data = from fun in engine.Esmart_Sys_Functions
                       join menu in engine.Esmart_Sys_Navigation_Function
                       on fun.FunctionId equals menu.FunctionId
                       join roles in engine.Esmart_Sys_Role_Navigation_Function
                       on menu.NavigationId equals roles.NavigationId
                       where fun.FunctionId == roles.FunctionId
                       && roleId.Contains(roles.RoleId)
                       select new MenuFunctionModel()
                       {
                           FunctionId = fun.FunctionId,
                           FunctionKey = fun.FunctionKey,
                           FunctionName = fun.FunctionName,
                           NavigationId = menu.NavigationId
                       };
            return data.ToList();
        }

        
        public static Esmart_Sys_Navigations GetModel(int id)
        {
            var engine = PermissionDb.CreateEngine();

            return engine.Esmart_Sys_Navigations.FirstOrDefault(a => a.NavigationId == id);
        }

        public static int Add(Esmart_Sys_Navigations model)
        {
            var engine = PermissionDb.CreateEngine();
            int max;
            try
            {
                max = engine.Esmart_Sys_Navigations.Max(a => a.NavigationId);
            }
            catch
            {
                max = 0;
            }
            model.NavigationId = max + 1;

            model.CreateTime = DateTime.Now;

            engine.Esmart_Sys_Navigations.Add(model);

            int code = engine.SaveChanges();
            if (code <= 0)
            {
                throw new TpoBaseException("新增异常");
            }
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = System.DateTime.Now, EventType = "add", OprTbName = "Esmart_Sys_Navigations", OprUserId = model.CreateId, OptDescription = string.Format(logFormat, model.CreateId,"添加", model.NavigationId), Remark = JsonConvert.SerializeObject(model) });
            CommonAction.ClearCache();

            return model.NavigationId;
        }

        public static int Update(Esmart_Sys_Navigations model)
        {
            var engine = PermissionDb.CreateEngine();

            engine.Esmart_Sys_Navigations.Attach(model);

            engine.Entry(model).State = EntityState.Modified;

            var code = engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = System.DateTime.Now, EventType = "update", OprTbName = "Esmart_Sys_Navigations", OprUserId = model.CreateId, OptDescription = string.Format(logFormat, model.CreateId,"修改", model.NavigationId), Remark = JsonConvert.SerializeObject(model) });
            CommonAction.ClearCache();

            return code;
        }

        public static int Delete(Esmart_Sys_Navigations model)
        {
            var engine = PermissionDb.CreateEngine();

            engine.Esmart_Sys_Navigations.Remove(model);

            int reInt = engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Navigations", OprUserId = model.CreateId, OptDescription = string.Format(logFormat, model.CreateId,"删除", model.NavigationId), Remark = JsonConvert.SerializeObject(model) });
            CommonAction.ClearCache();

            return reInt;
        }

        public static List<Esmart_Sys_Navigations> GetListByParentId(int parentId)
        {
            var engine = PermissionDb.CreateEngine();
            var list = engine.Esmart_Sys_Navigations.Where(m => m.ParentID == parentId);
            return list.ToList();
        }

        public static int DelByList(List<int> list,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            var navigation = engine.Esmart_Sys_Navigations.Where(m => list.Contains(m.NavigationId));
            engine.Esmart_Sys_Navigations.RemoveRange(navigation);
            sb.Append(JsonConvert.SerializeObject(navigation)).Append(";");

            var navigationFunction = engine.Esmart_Sys_Navigation_Function.Where(m => list.Contains(m.NavigationId));
            engine.Esmart_Sys_Navigation_Function.RemoveRange(navigationFunction);
            sb.Append(JsonConvert.SerializeObject(navigationFunction)).Append(";");

            var role_navs = engine.Esmart_Sys_Role_Navigations.Where(n => list.Contains(n.NavigationId));
            engine.Esmart_Sys_Role_Navigations.RemoveRange(role_navs);
            sb.Append(JsonConvert.SerializeObject(role_navs)).Append(";");

            var role_nav_functions = engine.Esmart_Sys_Role_Navigation_Function.Where(n => list.Contains(n.NavigationId));
            engine.Esmart_Sys_Role_Navigation_Function.RemoveRange(role_nav_functions);
            sb.Append(JsonConvert.SerializeObject(role_nav_functions)).Append(";");

            var user_navs = engine.Esmart_Sys_User_Navigations.Where(n => list.Contains(n.NavigationId));
            engine.Esmart_Sys_User_Navigations.RemoveRange(user_navs);
            sb.Append(JsonConvert.SerializeObject(user_navs)).Append(";");

            var user_nav_functions = engine.Esmart_Sys_User_Navigation_Function.Where(n => list.Contains(n.NavigationId));
            engine.Esmart_Sys_User_Navigation_Function.RemoveRange(user_nav_functions);
            sb.Append(JsonConvert.SerializeObject(user_nav_functions)).Append(";");
            int count = engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Navigations,Esmart_Sys_Navigation_Function,Esmart_Sys_Role_Navigations,Esmart_Sys_Role_Navigation_Function,Esmart_Sys_User_Navigations,Esmart_Sys_User_Navigation_Function", OprUserId = optUserId, OptDescription = string.Format("用户：{0}批量删除了菜单及其关系表,菜单ID：{1}", optUserId, string.Join(",", navigation.Select(s => s.NavigationId))), Remark = sb.ToString() });

            return count;
        }



        ///////////////////////////

        public static List<MenuFunctionModel> GetUserAddAllMensFunctions(List<int> userId)
        {
            var engine = PermissionDb.CreateEngine();
            var data = from fun in engine.Esmart_Sys_Functions
                       join menu in engine.Esmart_Sys_Navigation_Function
                       on fun.FunctionId equals menu.FunctionId
                       join users in engine.Esmart_Sys_User_Navigation_Function
                       on menu.NavigationId equals users.NavigationId
                       where fun.FunctionId == users.FunctionId
                       && userId.Contains(users.UserId)
                       select new MenuFunctionModel()
                       {
                           FunctionId = fun.FunctionId,
                           FunctionKey = fun.FunctionKey,
                           FunctionName = fun.FunctionName,
                           NavigationId = menu.NavigationId
                       };
            return data.ToList();
        }
    }
}
