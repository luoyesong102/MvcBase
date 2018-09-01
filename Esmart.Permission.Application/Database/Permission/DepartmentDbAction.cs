using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Newtonsoft.Json;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Caching;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Data
{
    public class DepartmentDbAction 
    {
        private const string logFormat="用户：{0}{1}了部门,ID：{2}";
        public const string guid = "Esmart.Permission.Application.Data.DepartmentDbAction";
        /// <summary>
        /// 根据ID获取部门信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Esmart_Sys_Departments GetDeparentById(int id)
        {
            var engine = PermissionDb.CreateEngine().Esmart_Sys_Departments;
            return engine.SingleOrDefault(d => d.DeparentId == id);
        }

        #region 添加，删除，修改

        public static void Add(Esmart_Sys_Departments model)
        {
            var engine = PermissionDb.CreateEngine();
            model.DeparentId = (engine.Esmart_Sys_Departments.Max(n => (int?)n.DeparentId) ?? 0) + 1;
            engine.Esmart_Sys_Departments.Add(model);
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = DateTime.Now, EventType = "add", OprTbName = "Esmart_Sys_Departments", OprUserId = model.CreateId, OptDescription = string.Format(logFormat, model.CreateId, System.DateTime.Now, "添加", model.DeparentId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(model) });
            CommonAction.ClearCache();
        }

        public static int Update(Esmart_Sys_Departments model)
        {
            var engine = PermissionDb.CreateEngine();

            DbEntityEntry<Esmart_Sys_Departments> entry = engine.Entry(model);
            entry.State = EntityState.Modified;
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = DateTime.Now, EventType = "update", OprTbName = "Esmart_Sys_Departments", OprUserId = model.CreateId, OptDescription = string.Format(logFormat, model.CreateId, System.DateTime.Now, "修改", model.DeparentId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(model) });
            int result = engine.SaveChanges();
            CommonAction.ClearCache();           
            return result;
        }

        public static int Delete(int deparentId)
        {
            int result = -1;
            var engine = PermissionDb.CreateEngine();
            Dictionary<string, List<object>> dctTemp = new Dictionary<string, List<object>>();
            if (engine.Esmart_Sys_Departments.Any(n => n.DeparentId == deparentId))
            {
                var all = engine.Esmart_Sys_Departments.Where(n => n.IsDelete == 0).OrderBy(n => n.ParentId).ToArray();
                var department = all.FirstOrDefault(n => n.DeparentId == deparentId);
                var dic = new Dictionary<int, Esmart_Sys_Departments>(all.Length);

                //构建部门树形结构

                Esmart_Sys_Departments department2;
                foreach (var item in all)
                {
                    dic[item.DeparentId] = item;
                    if (dic.TryGetValue(item.ParentId, out department2))
                    {
                        if (department2.Children == null)
                            department2.Children = new List<Esmart_Sys_Departments>(20);
                        department2.Children.Add(item);
                    }
                }

                //删除部门及所有下级部门
                DeleteDepartment(engine, department,ref dctTemp);

                result = engine.SaveChanges();
                if (dctTemp.Count > 0)
                {
                    System.Text.StringBuilder sbRemark = new System.Text.StringBuilder();
                    foreach (string key in dctTemp.Keys)
                    {
                        switch (key)
                        {
                            case "role":
                                sbRemark.Append("删除的角色：").Append(JsonConvert.SerializeObject(dctTemp[key])).Append(";");
                                break;
                            case "user":
                                sbRemark.Append("删除的用户：").Append(JsonConvert.SerializeObject(dctTemp[key])).Append(";");
                                break;
                            case "department":
                                sbRemark.Append("删除的部门：").Append(JsonConvert.SerializeObject(dctTemp[key])).Append(";");
                                break;
                        }
                    }


                    LogHelper<DepartmentRequest>.LogInstance(guid, new Action<UserLiteDto, DepartmentRequest>((users, depart) =>
                    {
                        RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = users.UserID, CreateTime = DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Departments,Esmart_Sys_Deparent_Role,Esmart_Sys_Department_User", OprUserId = users.UserID, OptDescription = string.Format(logFormat, users.UserID, System.DateTime.Now, "删除", deparentId) + "；其中包括部门角色、部门用户、部门信息", Remark = sbRemark.ToString() });
                    }));
                    //RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = userId, CreateTime = DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Departments,Esmart_Sys_Deparent_Role,Esmart_Sys_Department_User", OprUserId = userId, OptDescription = string.Format(logFormat, userId, System.DateTime.Now, "删除", deparentId) + "；其中包括部门角色、部门用户、部门信息", Remark = sbRemark.ToString() });

                }
                CommonAction.ClearCache();
            }


            return result;
        }

        /// <summary>
        /// 删除部门及所有下级部门 -- 软删除
        /// </summary>
        private static void DeleteDepartment(PermissionContext dbContext, Esmart_Sys_Departments department,ref Dictionary<string,List<object>> dctDel)
        {
            department.IsDelete = 1;
            //删除部门-角色关联
            var deparentRoles = dbContext.Esmart_Sys_Deparent_Role.Where(a => a.DeparentId == department.DeparentId).ToArray();
            dbContext.Esmart_Sys_Deparent_Role.RemoveRange(deparentRoles);
            if(!dctDel.ContainsKey("role"))
            {
                dctDel.Add("role", new List<object>{});
            }
            if (deparentRoles.Count() > 0)
            {
                dctDel["role"].Add(deparentRoles);
            }
            //删除部门-用户关联
            var deparentUsers = dbContext.Esmart_Sys_Department_User.Where(a => a.DeparentId == department.DeparentId).ToArray();
            dbContext.Esmart_Sys_Department_User.RemoveRange(deparentUsers);
            if (!dctDel.ContainsKey("user"))
            {
                dctDel.Add("user", new List<object> { });
            }
            if (deparentUsers.Count() > 0)
            {
                dctDel["user"].Add(deparentUsers);
            }
            if (!dctDel.ContainsKey("department"))
            {
                dctDel.Add("department", new List<object> { });
            }
            dctDel["department"].Add(department);
            if (department.Children == null || department.Children.Count == 0)
                return;

            foreach (var child in department.Children)
            {
                DeleteDepartment(dbContext, child, ref dctDel);
            }
        }

        #endregion

        public static SoaDataPageResponse<UsersView> GetUsersByDepartList(SoaDataPage<UserSearchModel> filter)
        {
            var engine = PermissionDb.CreateEngine();

            var query = from t in engine.Esmart_Sys_Users
                        join d in engine.Esmart_Sys_Department_User on t.UserID equals d.UserId
                        join dep in engine.Esmart_Sys_Departments on d.DeparentId equals dep.DeparentId
                        where d.DeparentId == filter.Where.DeapartmentId
                        select new UsersView
                        {
                            UserID = t.UserID,
                            WorkNo = t.WorkNo,
                            UserName = t.UserName,
                            PassWord = t.PassWord,
                            Email = t.Email,
                            Ename = t.Ename,
                            TrueName = t.TrueName,
                            Sex = t.Sex,
                            Education = t.Education,
                            Graduate = t.Graduate,
                            Birthday = t.Birthday,
                            qq = t.qq,
                            Skype = t.Skype,
                            Mobile = t.Mobile,
                            HomeTel = t.HomeTel,
                            HomeAddr = t.HomeAddr,
                            OfficeAddr = t.OfficeAddr,
                            OfficeName = t.OfficeName,
                            Isleave = t.Isleave,
                            IsDelete = t.IsDelete,
                            Remark = t.Remark,
                            CreateId = t.CreateId,
                            CreateTime = d.CreateTime,
                            DeparentName = dep.Name
                        };

            if (!string.IsNullOrWhiteSpace(filter.Where.TrueName))
                query = query.Where(n => n.TrueName.Contains(filter.Where.TrueName));

            if (!string.IsNullOrWhiteSpace(filter.Where.Ename))
                query = query.Where(n => n.Ename.Contains(filter.Where.Ename));

            if (!string.IsNullOrWhiteSpace(filter.Where.WorkNo))
                query = query.Where(n => n.WorkNo.Contains(filter.Where.WorkNo));

            var response = new SoaDataPageResponse<UsersView> { Count = query.Count() };

            query = !string.IsNullOrWhiteSpace(filter.OrderBy) ? query.SortBy(filter.OrderBy + " " + filter.SortCol) : query.OrderByDescending(n => n.CreateTime);

            response.Body = query.Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).ToList();

            return response;
        }

        /// <summary>
        /// 根据用户Id获取用户所在部门及其所有下级部门列表
        /// </summary>
        public static IEnumerable<Esmart_Sys_Departments> GetDepartments(int? userId)
        {
            var engine = PermissionDb.CreateEngine();
            var departmentDic = GetDepartmentDic(engine);
            var result = new List<Esmart_Sys_Departments>(10);

            if (userId.HasValue)
            {
                var resultSet = new HashSet<int>();
                var userDepartmentIds = engine.Esmart_Sys_Department_User.Where(n => n.UserId == userId).Select(n => n.DeparentId).ToList();
                foreach (var depId in userDepartmentIds)
                {
                    Esmart_Sys_Departments department;
                    if (resultSet.Contains(depId) || !departmentDic.TryGetValue(depId, out department))
                        continue;
                    GetAllChildId(department, resultSet);
                    result.Add(department);
                }
            }
            else
            {
                foreach (var kv in departmentDic)
                {
                    if (kv.Value.ParentId > 0) break;
                    result.Add(kv.Value);
                }
            }
            return result;
        }

        public static IEnumerable<Esmart_Sys_Departments> GetParentDepartments(int? userId)
        {
            var result = new List<Esmart_Sys_Departments>(10);

            if (userId.HasValue)
            {
                var resultSet = new HashSet<int>();
                var engine = PermissionDb.CreateEngine();
                var userDepartmentIds = engine.Esmart_Sys_Department_User.Where(n => n.UserId == userId).Select(n => n.DeparentId).ToList();
                var departmentDic = GetDepartmentDic(engine);
                foreach (var depId in userDepartmentIds)
                {
                    var parentId = depId;
                    while (parentId > 0)
                    {
                        Esmart_Sys_Departments department;
                        if (resultSet.Contains(parentId) || !departmentDic.TryGetValue(parentId, out department)) break;
                        result.Add(department);
                        resultSet.Add(parentId);
                        parentId = department.ParentId;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取部门所有下级部门Id哈希集合
        /// </summary>
        private static void GetAllChildId(Esmart_Sys_Departments department, HashSet<int> resultSet)
        {
            if (department.Children == null || department.Children.Count == 0)
                return;

            foreach (var child in department.Children)
            {
                resultSet.Add(child.DeparentId);
                GetAllChildId(child, resultSet);
            }
        }

        private static Dictionary<int, Esmart_Sys_Departments> GetDepartmentDic(PermissionContext engine)
        {
            var dic = CacheManager.CreateCache().Get<Dictionary<int, Esmart_Sys_Departments>>(CacheKey.Authorize_DepartmentDic);

            if (dic == null || dic.Count == 0)
            {
                //从线性数组转换为树形
                engine = engine ?? PermissionDb.CreateEngine();
                var linearList = engine.Esmart_Sys_Departments.Where(n => n.IsDelete == 0).OrderBy(n => n.ParentId).ToList();
                dic = new Dictionary<int, Esmart_Sys_Departments>(linearList.Count);
                foreach (var item in linearList)
                {
                    dic[item.DeparentId] = item;
                    Esmart_Sys_Departments department;
                    if (dic.TryGetValue(item.ParentId, out department))
                    {
                        if (department.Children == null)
                            department.Children = new List<Esmart_Sys_Departments>(20);
                        department.Children.Add(item);
                    }
                }
                CacheManager.CreateCache().Set(CacheKey.Authorize_DepartmentDic, dic);
            }
            return dic;
        }
    }
}
