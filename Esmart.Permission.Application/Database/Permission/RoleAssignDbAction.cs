using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.Data
{
    public class RoleAssignDbAction
    {
        public static int AssignDepartmentRoles(int departmentId, int creatId, List<int> roleIds)
        {
            var engine = PermissionDb.CreateEngine();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(),sbDesc=new System.Text.StringBuilder();
            //-----------------------------------------
            // 保存部门角色
            //-----------------------------------------
            var entities = engine.Esmart_Sys_Deparent_Role.Where(a => a.DeparentId == departmentId).ToList();
            engine.Esmart_Sys_Deparent_Role.RemoveRange(entities);
            sb.Append("删除部门角色：").Append(JsonConvert.SerializeObject(entities)).Append(";");
            foreach (var item in roleIds)
            {
                engine.Esmart_Sys_Deparent_Role.Add(new Esmart_Sys_Deparent_Role() { CreateId = creatId, CreateTime = DateTime.Now, RoleId = item, DeparentId = departmentId });
            }
            sb.Append("添加部门角色：").Append(JsonConvert.SerializeObject(engine.Esmart_Sys_Deparent_Role.ToList())).Append(";");
            //-----------------------------------------
            // 注入部门角色到用户角色列表中
            //-----------------------------------------
            var query = from user in engine.Esmart_Sys_Users
                        join dep_user in engine.Esmart_Sys_Department_User on user.UserID equals dep_user.UserId
                        where dep_user.DeparentId == departmentId
                        select user.UserID;

            foreach (var userId in query.ToArray())
            {
                var userRoleIds = engine.Esmart_Sys_User_Roles.Where(n => n.UserId == userId).Select(n => n.RoleId).ToArray();
                var exceptRoleIds = roleIds.Except(userRoleIds);
                foreach (var roleId in exceptRoleIds)
                {
                    engine.Esmart_Sys_User_Roles.Add(new Esmart_Sys_User_Roles() { CreateId = creatId, CreateTime = DateTime.Now, RoleId = roleId, UserId = userId });
                }
            }
            sb.Append("删除用户角色：").Append(JsonConvert.SerializeObject(engine.Esmart_Sys_User_Roles.ToList()));
            //-----------------------------------------
            // 保存并清空缓存
            //-----------------------------------------
            var result = engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = creatId, CreateTime = System.DateTime.Now, EventType = "add&delete", OprTbName = "Esmart_Sys_Deparent_Role,Esmart_Sys_User_Roles", OprUserId = creatId, OptDescription = string.Format("用户：{0}修改了部门角色、用户角色关系", creatId), Remark = sb.ToString() });
            CommonAction.ClearCache();
            return result;
        }

        public static List<FunctionResponse> GetFunctions(int appId, int roleId)
        {
            var engine = PermissionDb.CreateEngine();

            var allFunctions = engine.Esmart_Sys_Functions.Where(n => n.AppId == appId && n.IsDelete != 1)
                .Select(n => new FunctionResponse { Id = n.FunctionId, Name = n.FunctionName }).ToList();

            var roleFunctionIds = engine.Esmart_Sys_Role_Navigation_Function
                .Where(n => n.RoleId == roleId && n.NavigationId == 0)
                .Select(n => n.FunctionId).ToArray();

            allFunctions.ForEach(n => n.IsChoice = roleFunctionIds.Contains(n.Id));

            return allFunctions;
        }
    }
}
