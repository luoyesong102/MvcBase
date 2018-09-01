using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Data
{
    public class UserRolesDbAction
    {
        /// <summary>
        /// 删除当前角色Id对应的数据
        /// </summary>
        public static bool Delete(int roleId,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            var entities = engine.Esmart_Sys_User_Roles.Where(a => a.RoleId == roleId).ToList();
            engine.Esmart_Sys_User_Roles.RemoveRange(entities);
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_User_Roles", OprUserId = optUserId, OptDescription = string.Format("用户：{0}删除了用户角色关系,角色ID：{1}", optUserId, roleId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(entities) });
            CommonAction.ClearCache();
            return true;
        }

        public static List<Esmart_Sys_Roles> GetUserRolses(int userId)
        {
            var engine = PermissionDb.CreateEngine();
            var query = from role in engine.Esmart_Sys_Roles
                        join userRole in engine.Esmart_Sys_User_Roles on role.RoleId equals userRole.RoleId
                        where userRole.UserId == userId
                        select role;
            return query.ToList();
        }

        public static bool DeleteUserRoles(int userId, int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            var entities = engine.Esmart_Sys_User_Roles.Where(m => m.UserId == userId).ToList();
            engine.Esmart_Sys_User_Roles.RemoveRange(entities);
            var result = engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_User_Roles", OprUserId = optUserId, OptDescription = string.Format("用户：{0}删除了用户ID对应的角色关系,用户ID：{1}", optUserId, userId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(entities) });
            return result > 0;
        }

        public static int AddList(List<Esmart_Sys_User_Roles> list)
        {
            var engine = PermissionDb.CreateEngine();

            if (list != null && list.Any())
            {
                foreach (var lt in list)
                {
                    engine.Esmart_Sys_User_Roles.Add(lt);
                }
            }
            int updateCount= engine.SaveChanges();
            int optUserId=list.First().Id;
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy =optUserId , CreateTime = System.DateTime.Now, EventType = "add", OprTbName = "Esmart_Sys_User_Roles", OprUserId = optUserId, OptDescription = string.Format("用户：{0}新建了用户角色关系", optUserId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(list) });
            return updateCount;
        }

        public static List<Esmart_Sys_Roles> GetRolesByUserId(SoaDataPage<RoleGridFilterViewModel> fiter, out int count)
        {
            var data = PermissionDb.CreateEngine().Esmart_Sys_Roles.Where(a => a.CreateId == fiter.Where.LogInUserId);
            if (!string.IsNullOrEmpty(fiter.Where.Name))
            {
                data = data.Where(a => a.RoleName.Contains(fiter.Where.Name));
            }
            if (fiter.Where.CreateId != null)
            {
                data = data.Where(a => a.CreateId == fiter.Where.CreateId.Value);
            }
            count = data.Count();

            data = data.OrderByDescending(a => a.RoleId).Skip((fiter.PageIndex - 1) * fiter.PageSize).Take(fiter.PageSize);

            return data.ToList();
        }

        public static int RemoveUserRole(int userId, int roleId,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            var items = engine.Esmart_Sys_User_Roles.Where(n => n.UserId == userId && n.RoleId == roleId).ToArray();
            engine.Esmart_Sys_User_Roles.RemoveRange(items);
            int updateCount= engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_User_Roles", OprUserId = optUserId, OptDescription = string.Format("用户：{0}删除了用户角色关系,用户ID：{1},角色ID{2}", optUserId,userId,roleId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(items) });
            return updateCount;
        }

        public static List<DepartmentUserResponse2> GetUsersWithRole(int roleId)
        {
            var engine = PermissionDb.CreateEngine();

            var query = from user in engine.Esmart_Sys_Users
                        join role in engine.Esmart_Sys_User_Roles on user.UserID equals role.UserId
                        join depUser in engine.Esmart_Sys_Department_User on user.UserID equals depUser.UserId into depUser2
                        from depUser in depUser2.DefaultIfEmpty()
                        join dep in engine.Esmart_Sys_Departments on depUser.DeparentId equals dep.DeparentId into dep2
                        from dep in dep2.DefaultIfEmpty()
                        where role.RoleId == roleId && user.Isleave != 1
                        orderby depUser.DeparentId
                        orderby user.TrueName
                        select new DepartmentUserResponse2
                        {
                            DeparentName = dep.Name,
                            UserID = user.UserID,
                            TrueName = user.TrueName,
                            Ename = user.Ename,
                            Sex = user.Sex
                        };
            return query.ToList();
        }

        public static string[] GetUserRoleNames(int userId)
        {
            var engine = PermissionDb.CreateEngine();
            var query = from role in engine.Esmart_Sys_Roles
                        join userRole in engine.Esmart_Sys_User_Roles on role.RoleId equals userRole.RoleId
                        where userRole.UserId == userId
                        select role.RoleName;
            return query.ToArray();
        }
    }
}
