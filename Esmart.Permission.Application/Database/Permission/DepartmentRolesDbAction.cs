using System.Collections.Generic;
using System.Linq;
using System;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Permission.Application.Models.ControlModel;

namespace Esmart.Permission.Application.Data
{
    public class DepartmentRolesDbAction
    {
        public const string guid = "Esmart.Permission.Application.Data.DepartmentRolesDbAction";
        /// <summary>
        /// 删除角色Id对应的数据
        /// </summary>
        public static bool DeleteByRoleId(int roleId)
        {
            var engine = PermissionDb.CreateEngine();
            var entities = engine.Esmart_Sys_Deparent_Role.Where(a => a.RoleId == roleId).ToList();
            engine.Esmart_Sys_Deparent_Role.RemoveRange(entities);
            engine.SaveChanges();
            LogHelper<RoleModel>.LogInstance(guid, new Action<UserLiteDto, RoleModel>((users, depart) =>
            {
                RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = users.UserID, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Deparent_Role", OprUserId = users.UserID, Remark = Newtonsoft.Json.JsonConvert.SerializeObject(entities), OptDescription = string.Format("用户：{0}{1}了部门角色,ID：{2}", users.UserID, "删除", roleId) });
            }));
            
            CommonAction.ClearCache();
            return true;
        }

        /// <summary>
        /// 获取部门已经分配的角色
        /// </summary>
        public static List<Esmart_Sys_Roles> GetAssignedRolesOfDepartment(int departmentId)
        {
            var engine = PermissionDb.CreateEngine();
            var query = from role in engine.Esmart_Sys_Roles
                        join dep_role in engine.Esmart_Sys_Deparent_Role on role.RoleId equals dep_role.RoleId
                        where dep_role.DeparentId == departmentId
                        select role;
            return query.ToList();
        }
    }
}
