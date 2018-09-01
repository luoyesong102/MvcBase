using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Constants;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application
{
    public class RoleService : IRole
    {
        #region 获取学习顾问、排课顾问

        public List<UserLiteDto> GetStudyConsultant(int userId)
        {
            return GetConsultant(userId, true);
        }

        public List<UserLiteDto> GetPkConsultant(int userId)
        {
            return GetConsultant(userId, false);
        }

        //获取学习顾问或排课顾问
        private static List<UserLiteDto> GetConsultant(int userId, bool isGettingStudyConsultant)
        {
            return GetCurrentUser(userId);
        }

        //获取所有的学习顾问或排课顾问
        private static List<UserLiteDto> GetAllConsultant(IEnumerable<string> roleNames, bool isGettingStudyConsultant)
        {
            var engine = PermissionDb.CreateEngine();

            var query = from user in engine.Esmart_Sys_Users
                        join userRole in engine.Esmart_Sys_User_Roles on user.UserID equals userRole.UserId
                        join role in engine.Esmart_Sys_Roles on userRole.RoleId equals role.RoleId
                        where user.Isleave != 1 && user.IsDelete == 0
                            //&& !excludeUserIds.Contains(userRole.UserId)
                        && roleNames.Contains(role.RoleName)
                        select new UserLiteDto
                        {
                            UserID = user.UserID,
                            Ename = user.Ename,
                            TrueName = user.TrueName,
                            WorkNo = user.WorkNo,
                            Sex = user.Sex
                        };

            if (!isGettingStudyConsultant)
            {
                //11月24日 添加了排除 教务质监 角色的关联用户
                //12月1日  学习顾问不再排除教务质监
                var query1 = from userRole in engine.Esmart_Sys_User_Roles
                             join role in engine.Esmart_Sys_Roles on userRole.RoleId equals role.RoleId
                             where role.RoleName == BuiltinRoles.ScheduleConsultantAdmin
                             select userRole.UserId;

                var excludeUserIds = query1.ToArray();

                query = query.Where(n => !excludeUserIds.Contains(n.UserID));
            }

            var list = query.Distinct().ToList();
            //将超级管理员排除
            list.RemoveAll(n => CommonAction.IsSysAdmin(n.UserID));
            return list;
        }

        //获取当前用户信息
        private static List<UserLiteDto> GetCurrentUser(int userId)
        {
            var engine = PermissionDb.CreateEngine();

            var userlite = engine.Esmart_Sys_Users.Where(n => n.UserID == userId).Select(user => new UserLiteDto
            {
                UserID = user.UserID,
                Ename = user.Ename,
                TrueName = user.TrueName,
                WorkNo = user.WorkNo,
                Sex = user.Sex
            }).FirstOrDefault();

            return userlite == null ? new List<UserLiteDto>(0) : new List<UserLiteDto>() { userlite };
        }

        //获取组中的学习顾问
        private static List<UserLiteDto> GetGroupConsultant(int userId, bool isGettingStudyConsultant)
        {
            //-----------------------
            // 获取所有顾问
            //-----------------------
            var allConsultant = GetConsultant(-1, isGettingStudyConsultant);

            //-----------------------
            // 获取用户所属部门所有用户
            //-----------------------
            var engine = PermissionDb.CreateEngine();
            var dbDepartments = DepartmentDbAction.GetDepartments(userId);
            var departmentIds = new List<int>(100);
            GetUserDepartmentIds(dbDepartments, departmentIds);
            var query3 = from user in engine.Esmart_Sys_Users
                         join depUser in engine.Esmart_Sys_Department_User on user.UserID equals depUser.UserId
                         where departmentIds.Contains(depUser.DeparentId)
                         select new UserLiteDto
                         {
                             UserID = user.UserID,
                             Ename = user.Ename,
                             TrueName = user.TrueName,
                             WorkNo = user.WorkNo,
                             Sex = user.Sex
                         };

            //-----------------------
            // 取所有顾问和当前部门所有用户的交集并返回
            //-----------------------
            var depUsers = query3.ToArray();
            return allConsultant.Intersect(depUsers).ToList();
        }

        //将树形部门集合转换为线性列表
        private static void GetUserDepartmentIds(IEnumerable<Esmart_Sys_Departments> departments, List<int> result)
        {
            foreach (var item in departments)
            {
                result.Add(item.DeparentId);

                if (item.Children != null && item.Children.Count > 0)
                {
                    GetUserDepartmentIds(item.Children, result);
                }
            }
        }

        #endregion

        public List<UserLiteDto> GetUsersByRoleName(string roleName)
        {
            var engine = PermissionDb.CreateEngine();

            var query = from user in engine.Esmart_Sys_Users
                        join userRole in engine.Esmart_Sys_User_Roles on user.UserID equals userRole.UserId
                        join role in engine.Esmart_Sys_Roles on userRole.RoleId equals role.RoleId
                        where role.RoleName == roleName && user.Isleave != 1 && user.IsDelete != 1
                        select new UserLiteDto
                        {
                            UserID = user.UserID,
                            Ename = user.Ename,
                            TrueName = user.TrueName,
                            WorkNo = user.WorkNo
                        };

            return query.ToList();
        }

        public List<UserLiteDto> GetStudyConsultantLeader(int userId)
        {
            var engine = PermissionDb.CreateEngine();

            var depId = engine.Esmart_Sys_Department_User.Where(n => n.UserId == userId).Select(n => n.DeparentId).FirstOrDefault();

            //所给用户不属于任何部门
            if (depId < 1) return new List<UserLiteDto>(0);

            var query = from userRole in engine.Esmart_Sys_User_Roles
                        join role in engine.Esmart_Sys_Roles on userRole.RoleId equals role.RoleId
                        where userRole.UserId == userId && role.RoleName == BuiltinRoles.StudyConsultantAdmin
                        select userRole.UserId;

            //所给用户不是学习顾问
            if (!query.Any()) return new List<UserLiteDto>(0);

            //获取所有的上级部门
            var parentDepartmentIds = DepartmentDbAction.GetParentDepartments(userId).Select(n => n.DeparentId).ToList();

            var query2 = from user in engine.Esmart_Sys_Users
                         join depUser in engine.Esmart_Sys_Department_User on user.UserID equals depUser.UserId
                         join userRole in engine.Esmart_Sys_User_Roles on user.UserID equals userRole.UserId
                         join role in engine.Esmart_Sys_Roles on userRole.RoleId equals role.RoleId
                         where user.IsDelete != 1 && user.Isleave != 1
                             //本组学习顾问组长或值班组长、上级部门的学习顾问主管、学习顾问经理
                            && ((depUser.DeparentId == depId && (role.RoleName == BuiltinRoles.StudyConsultantAdmin || role.RoleName == BuiltinRoles.StudyConsultantAdmin))
                            || (parentDepartmentIds.Contains(depId) && role.RoleName == BuiltinRoles.StudyConsultantAdmin)
                            || role.RoleName == BuiltinRoles.StudyConsultantAdmin)
                         select new UserLiteDto
                         {
                             UserID = user.UserID,
                             Ename = user.Ename,
                             TrueName = user.TrueName,
                             WorkNo = user.WorkNo
                         };



            return query2.Distinct().ToList();
        }

        public List<UserLiteDto> GetScheduleConsultantLeader(int userId)
        {
            var engine = PermissionDb.CreateEngine();

            var depId = engine.Esmart_Sys_Department_User.Where(n => n.UserId == userId).Select(n => n.DeparentId).FirstOrDefault();

            //所给用户不属于任何部门
            if (depId < 1) return new List<UserLiteDto>(0);

            var query = from userRole in engine.Esmart_Sys_User_Roles
                        join role in engine.Esmart_Sys_Roles on userRole.RoleId equals role.RoleId
                        where userRole.UserId == userId && role.RoleName == BuiltinRoles.StudyConsultantAdmin
                        select userRole.UserId;

            //所给用户不是排课顾问
            if (!query.Any()) return new List<UserLiteDto>(0);

            //返回学习顾问组长列表
            var query2 = from user in engine.Esmart_Sys_Users
                         join depUser in engine.Esmart_Sys_Department_User on user.UserID equals depUser.UserId
                         join userRole in engine.Esmart_Sys_User_Roles on user.UserID equals userRole.UserId
                         join role in engine.Esmart_Sys_Roles on userRole.RoleId equals role.RoleId
                         where user.IsDelete != 1 && user.Isleave != 1 && depUser.DeparentId == depId
                               && role.RoleName == BuiltinRoles.StudyConsultantAdmin
                         select new UserLiteDto
                         {
                             UserID = user.UserID,
                             Ename = user.Ename,
                             TrueName = user.TrueName,
                             WorkNo = user.WorkNo
                         };
            return query2.ToList();
        }

        public List<UserLiteDto> GetUsersByFunctionKey(string functionKey, int userId)
        {
            var engine = PermissionDb.CreateEngine();

            var funcId = engine.Esmart_Sys_Functions.Where(n => n.FunctionKey == functionKey).Select(n => n.FunctionId).FirstOrDefault();

            if (funcId < 1) return new List<UserLiteDto>(0);

            var query1 = (from user in engine.Esmart_Sys_Users
                         join userRole in engine.Esmart_Sys_User_Roles on user.UserID equals userRole.UserId
                         join roleFunc in engine.Esmart_Sys_Role_Navigation_Function on userRole.RoleId equals roleFunc.RoleId
                         where user.Isleave != 1 && user.IsDelete != 1 && roleFunc.FunctionId == funcId
                         select new UserLiteDto
                         {
                             UserID = user.UserID,
                             Ename = user.Ename,
                             TrueName = user.TrueName,
                             WorkNo = user.WorkNo
                         }).Union(
                         from user in engine.Esmart_Sys_Users
                         join userFunc in engine.Esmart_Sys_User_Navigation_Function on user.UserID equals userFunc.UserId
                         where user.Isleave != 1 && user.IsDelete != 1 && userFunc.FunctionId == funcId
                         select new UserLiteDto
                         {
                             UserID = user.UserID,
                             Ename = user.Ename,
                             TrueName = user.TrueName,
                             WorkNo = user.WorkNo
                         });
            List<UserLiteDto> lstUsers = query1.ToList();
            var allUsers = lstUsers.Where((x,i) =>lstUsers.FindIndex(f=>f.UserID==x.UserID)==i ).ToList();
           
            if (userId > 0)
            {
                //-----------------------
                // 获取用户所属部门所有用户
                //-----------------------
                var dbDepartments = DepartmentDbAction.GetDepartments(userId);
                var departmentIds = new List<int>(100);
                GetUserDepartmentIds(dbDepartments, departmentIds);
                var query2 = from user in engine.Esmart_Sys_Users
                             join depUser in engine.Esmart_Sys_Department_User on user.UserID equals depUser.UserId
                             where departmentIds.Contains(depUser.DeparentId)
                             select new UserLiteDto
                             {
                                 UserID = user.UserID,
                                 Ename = user.Ename,
                                 TrueName = user.TrueName,
                                 WorkNo = user.WorkNo,
                                 Sex = user.Sex
                             };
                var depUsers = query2.ToArray();
                return allUsers.Intersect(depUsers).ToList();
            }

            return allUsers;
        }

        public List<UserLiteDto> GetAllUsersByFunctionKey(string functionKey,int appId, int menuId=0)
        {
            var engine = PermissionDb.CreateEngine();

            var funcId = engine.Esmart_Sys_Functions.Where(n => n.FunctionKey == functionKey && n.AppId==appId).Select(n => n.FunctionId).FirstOrDefault();

            if (funcId < 1) return new List<UserLiteDto>(0);

            var query1 = (from user in engine.Esmart_Sys_Users
                         join userRole in engine.Esmart_Sys_User_Roles on user.UserID equals userRole.UserId
                         join roleFunc in engine.Esmart_Sys_Role_Navigation_Function on userRole.RoleId equals roleFunc.RoleId
                         where user.Isleave != 1 && user.IsDelete != 1 && roleFunc.FunctionId == funcId && (menuId>0?roleFunc.NavigationId==menuId:true)
                         select new UserLiteDto
                         {
                             UserID = user.UserID,
                             Ename = user.Ename,
                             TrueName = user.TrueName,
                             WorkNo = user.WorkNo
                         }).Union(
                        from user in engine.Esmart_Sys_Users
                         join userFunc in engine.Esmart_Sys_User_Navigation_Function on user.UserID equals userFunc.UserId
                         where user.Isleave != 1 && user.IsDelete != 1 && userFunc.FunctionId == funcId
                         select new UserLiteDto
                         {
                             UserID = user.UserID,
                             Ename = user.Ename,
                             TrueName = user.TrueName,
                             WorkNo = user.WorkNo
                         });
            List<UserLiteDto> lstUsers = query1.ToList();
            var allUsers = lstUsers.Where((x, i) => lstUsers.FindIndex(f => f.UserID == x.UserID) == i).ToList();

            return allUsers;
        }

        public List<UserLiteDto> GetGroupUsersByFunctionKey(string functionKey, int userId, int menuId, int appId=0)
        {
            var engine = PermissionDb.CreateEngine();

           // var funcId = engine.Esmart_Sys_Functions.Where(n => n.FunctionKey == functionKey && n.).Select(n => n.FunctionId).FirstOrDefault();

            //if (funcId < 1) return new List<UserLiteDto>(0);

            var query1 = (from user in engine.Esmart_Sys_Users
                          join userRole in engine.Esmart_Sys_User_Roles on user.UserID equals userRole.UserId
                          join roleFunc in engine.Esmart_Sys_Role_Navigation_Function on userRole.RoleId equals roleFunc.RoleId
                          join func in engine.Esmart_Sys_Functions on roleFunc.FunctionId equals func.FunctionId
                          where user.Isleave != 1 && user.IsDelete != 1 && func.FunctionKey==functionKey && (menuId > 0 ? roleFunc.NavigationId == menuId : true)
                          select new UserLiteDto
                          {
                              UserID = user.UserID,
                              Ename = user.Ename,
                              TrueName = user.TrueName,
                              WorkNo = user.WorkNo
                          }).Union(
                        from user in engine.Esmart_Sys_Users
                        join userFunc in engine.Esmart_Sys_User_Navigation_Function on user.UserID equals userFunc.UserId
                        join func in engine.Esmart_Sys_Functions on userFunc.FunctionId equals func.FunctionId
                        where user.Isleave != 1 && user.IsDelete != 1 && func.FunctionKey==functionKey && (menuId > 0 ? userFunc.NavigationId == menuId : true)
                        select new UserLiteDto
                        {
                            UserID = user.UserID,
                            Ename = user.Ename,
                            TrueName = user.TrueName,
                            WorkNo = user.WorkNo
                        });
            
           var groupUsers= DepartmentUserDbAction.GetGroupsByUserId(userId);
           var filterto = groupUsers.ConvertAll(c => c.UserID);
           List<UserLiteDto> lstUsers =CommonAction.IsSysAdmin(userId)?query1.ToList(): query1.Where(w => filterto.Contains(w.UserID)).ToList();
            var allUsers = lstUsers.Where((x, i) => lstUsers.FindIndex(f => f.UserID == x.UserID) == i).ToList();

            return allUsers;
        }
        //public List<DepartmentResponse> GetGroupUsersByLoginUserID(int userId)
        //{

        //    return DepartmentUserDbAction.GetGroupsByUserId(userId);
        //}
    }
}


