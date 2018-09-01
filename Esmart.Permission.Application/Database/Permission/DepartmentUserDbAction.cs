using System;
using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Data
{
    public class DepartmentUserDbAction
    {
        public static bool DelDepartmentRelation(int userId, int departMentId,int currUser=0)
        {
            var engine = PermissionDb.CreateEngine();
            var departUsers = engine.Esmart_Sys_Department_User.Where(a => a.UserId == userId && a.DeparentId == departMentId).ToList();
            engine.Esmart_Sys_Department_User.RemoveRange(departUsers);
             bool isDelSuccess =engine.SaveChanges() > 0;
             RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = currUser, CreateTime = DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Department_User",OprUserId=currUser,OptDescription=string.Format("用户：{0}删除了部门关系用户,用户ID：{1},部门ID：{2}",currUser,userId,departMentId),Remark=Newtonsoft.Json.JsonConvert.SerializeObject(departUsers) });
             return isDelSuccess;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Add(Esmart_Sys_Department_User model)
        {
            var engine = PermissionDb.CreateEngine();
            engine.Esmart_Sys_Department_User.Add(model);
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = model.CreateTime, EventType = "add", OprTbName = "Esmart_Sys_Department_User", OprUserId = model.CreateId, OptDescription = string.Format("用户：{0}添加了部门关系用户,用户ID：{1},部门ID：{2}", model.CreateId, model.UserId, model.DeparentId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(model) });
            CommonAction.ClearCache();
            return true;
        }

        /// <summary>
        /// 根据部门Id获取部门中的用户
        /// </summary>
        public static IEnumerable<UsersView> GetUsersInDepartment(int departmentId, bool withLeaves)
        {
            var engine = PermissionDb.CreateEngine();
            var query = from user in engine.Esmart_Sys_Users
                        join dep_user in engine.Esmart_Sys_Department_User on user.UserID equals dep_user.UserId
                        join dep in engine.Esmart_Sys_Departments on dep_user.DeparentId equals dep.DeparentId
                        where dep_user.DeparentId == departmentId
                        select new UsersView
                        {
                            UserID = user.UserID,
                            WorkNo = user.WorkNo,
                            UserName = user.UserName,
                            PassWord = user.PassWord,
                            Email = user.Email,
                            Ename = user.Ename,
                            TrueName = user.TrueName,
                            Sex = user.Sex,
                            Education = user.Education,
                            Graduate = user.Graduate,
                            Birthday = user.Birthday,
                            qq = user.qq,
                            Skype = user.Skype,
                            Mobile = user.Mobile,
                            HomeTel = user.HomeTel,
                            HomeAddr = user.HomeAddr,
                            OfficeAddr = user.OfficeAddr,
                            OfficeName = user.OfficeName,
                            Isleave = user.Isleave,
                            IsDelete = user.IsDelete,
                            Remark = user.Remark,
                            CreateId = user.CreateId,
                            CreateTime = user.CreateTime,
                            DeparentName = dep.Name
                        };
            if (!withLeaves)
            {
                query = query.Where(n => n.Isleave != 1);
            }
            return query.ToArray();
        }

        public static List<DepartmentUserResponse> GetUserOutDepartment(int departmentId, int loginUserId, bool isSysAdmin)
        {
            var engine = PermissionDb.CreateEngine();

            if (isSysAdmin)
            {
                var query = from user in engine.Esmart_Sys_Users
                            join depUser in engine.Esmart_Sys_Department_User on user.UserID equals depUser.UserId into depUser2
                            from depUser in depUser2.DefaultIfEmpty()
                            join depart in engine.Esmart_Sys_Departments on depUser.DeparentId equals depart.DeparentId into depart2
                            from depart in depart2.DefaultIfEmpty()
                            where depUser.DeparentId != departmentId && user.Isleave == 0 && user.IsDelete == 0
                            orderby depUser.DeparentId
                            select new DepartmentUserResponse
                            {
                                UserID = user.UserID,
                                UserName = "(" + depart.Name + ")" + user.TrueName + user.Ename
                            };

                return query.ToList();
            }

            //-----------------------------------
            // 获取用户所在部门
            //-----------------------------------

            var userDepartmentId = engine.Esmart_Sys_Department_User.Where(n => n.UserId == loginUserId).Select(n => n.DeparentId).FirstOrDefault();
            if (userDepartmentId < 1) return new List<DepartmentUserResponse>(0);

            //-----------------------------------
            // 获取用户所在部门所有子部门的部门id集合
            //-----------------------------------

            var ids = new List<int>(20);
            var tempIds = engine.Esmart_Sys_Departments.Where(n => n.IsDelete == 0 && n.ParentId == userDepartmentId).Select(n => n.DeparentId).ToArray();
            while (tempIds.Length > 0)
            {
                ids.AddRange(tempIds);
                tempIds = engine.Esmart_Sys_Departments.Where(n => tempIds.Contains(n.ParentId)).Select(n => n.DeparentId).ToArray();
            }

            //用户部门不等于选定部门
            if (userDepartmentId != departmentId)
            {
                ids.Remove(departmentId);
                ids.Add(userDepartmentId);
            }

            //-----------------------------------
            // 获取所有子部门的用户集合
            //-----------------------------------

            var query2 = from user in engine.Esmart_Sys_Users
                         join depUser in engine.Esmart_Sys_Department_User on user.UserID equals depUser.UserId
                         join depart in engine.Esmart_Sys_Departments on depUser.DeparentId equals depart.DeparentId
                         where user.Isleave == 0 && user.IsDelete == 0 && ids.Contains(depUser.DeparentId)
                         orderby depUser.DeparentId
                         select new DepartmentUserResponse
                         {
                             UserID = user.UserID,
                             UserName = "(" + depart.Name + ")" + user.TrueName + user.Ename
                         };

            var query3 = from user in engine.Esmart_Sys_Users
                         where user.Isleave == 0 && user.IsDelete == 0 && !engine.Esmart_Sys_Department_User.Any(n => n.UserId == user.UserID)
                         select new DepartmentUserResponse
                         {
                             UserID = user.UserID,
                             UserName = "()" + user.TrueName + user.Ename
                         };

            return query2.Concat(query3).ToList();
        }

        public static int ChangeDepartmentOfUser(List<int> userIds, int newDepartmentId, int createId)
        {
            var engine = PermissionDb.CreateEngine();

            if (!engine.Esmart_Sys_Departments.Any(n => n.DeparentId == newDepartmentId))
                throw new TpoBaseException("调入部门已不存在，请刷新后重试");
            List<Esmart_Sys_Department_User> delDepartUser = new List<Esmart_Sys_Department_User>(), addDepartUser = new List<Esmart_Sys_Department_User>();
            foreach (var userId in userIds)
            {
                if (!engine.Esmart_Sys_Users.Any(n => n.UserID == userId))
                    throw new TpoBaseException("用户已不存在，请刷新后重试");

                var depUsers = engine.Esmart_Sys_Department_User.Where(n => n.UserId == userId).ToArray();
                engine.Esmart_Sys_Department_User.RemoveRange(depUsers);
                delDepartUser.AddRange(depUsers);
                var newModel = new Esmart_Sys_Department_User
                {
                    UserId = userId,
                    DeparentId = newDepartmentId,
                    CreateId = createId,
                    CreateTime = DateTime.Now
                };
                engine.Esmart_Sys_Department_User.Add(newModel);
                addDepartUser.Add(newModel);
            }
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = createId, CreateTime = DateTime.Now, EventType = "add&delete", OprTbName = "Esmart_Sys_Department_User", OprUserId = createId, OptDescription = string.Format("用户：{0}更新了部门用户关系,更新的用户ID：{1},原部门ID{2},新部门ID：{3}", createId, string.Join(",", userIds), string.Join(",", delDepartUser.Select(s => s.DeparentId)),string.Join(",",addDepartUser.Select(s=>s.DeparentId))), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(delDepartUser)+";"+Newtonsoft.Json.JsonConvert.SerializeObject(addDepartUser) });
            var result = engine.SaveChanges();
            CommonAction.ClearCache();
            return result;
        }

        public static IEnumerable<UsersView> GetAllUsers(bool withLeaves)
        {
            var engine = PermissionDb.CreateEngine();

            var query = from user in engine.Esmart_Sys_Users
                        join dep_user in engine.Esmart_Sys_Department_User on user.UserID equals dep_user.UserId into x
                        from dep_user2 in x.DefaultIfEmpty()
                        join dep in engine.Esmart_Sys_Departments on dep_user2.DeparentId equals dep.DeparentId into y
                        from dep2 in y.DefaultIfEmpty()
                        select new UsersView
                        {
                            UserID = user.UserID,
                            WorkNo = user.WorkNo,
                            UserName = user.UserName,
                            PassWord = user.PassWord,
                            Email = user.Email,
                            Ename = user.Ename,
                            TrueName = user.TrueName,
                            Sex = user.Sex,
                            Education = user.Education,
                            Graduate = user.Graduate,
                            Birthday = user.Birthday,
                            qq = user.qq,
                            Skype = user.Skype,
                            Mobile = user.Mobile,
                            HomeTel = user.HomeTel,
                            HomeAddr = user.HomeAddr,
                            OfficeAddr = user.OfficeAddr,
                            OfficeName = user.OfficeName,
                            Isleave = user.Isleave,
                            IsDelete = user.IsDelete,
                            Remark = user.Remark,
                            CreateId = user.CreateId,
                            CreateTime = user.CreateTime,
                            DeparentName = dep2.Name
                        };
            if (!withLeaves)
            {
                query = query.Where(n => n.Isleave != 1);
            }
            return query.ToArray();
        }

        public static List<UsersView> GetGroupsByUserId(int userId)
        {
            var engine = PermissionDb.CreateEngine();
            
            var groupUsers=from d in engine.Esmart_Sys_Departments join du in engine.Esmart_Sys_Department_User on d.DeparentId equals du.DeparentId
                           join u in engine.Esmart_Sys_Users on du.UserId equals u.UserID 
                           where d.IsDelete==0 && u.IsDelete==0 && u.Isleave==0
                          && engine.Esmart_Sys_Department_User.Any(a=>a.DeparentId==du.DeparentId && a.UserId==userId)
                           && !engine.Esmart_Sys_Departments.Any(a=>d.DeparentId==a.ParentId)
                           select new UsersView{UserID=u.UserID,WorkNo=u.WorkNo,TrueName=u.TrueName,Sex=u.Sex,Email=u.Email,Ename=u.Ename,DeparentId=d.DeparentId,DeparentName=d.Name};
            List<UsersView> lstResult = groupUsers.ToList();
            return lstResult.Where((x,i) => lstResult.FindIndex(f=>f.UserID==x.UserID)==i).ToList();
        }
    }
}
