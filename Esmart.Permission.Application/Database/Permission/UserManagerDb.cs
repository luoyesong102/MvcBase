using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Permission.Application.Services;
using Esmart.Permission.Application.Utilities;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Data
{
    public class UserManagerDb
    {
        public static List<RoleResponse> GetRolesByUserId(int userId)
        {
            var engine = PermissionDb.CreateEngine();

            var roles = from userRole in engine.Esmart_Sys_User_Roles
                        join role in engine.Esmart_Sys_Roles on userRole.RoleId equals role.RoleId
                        where role.StartTime <= DateTime.Now && role.EndTime >= DateTime.Now && userRole.UserId == userId
                        select new RoleResponse
                        {
                            RoleId = userRole.RoleId,
                            RoleName = role.RoleName
                        };

            return roles.ToList();
        }

        public static List<string> GetRolesByUser(int userId)
        {
            var engine = PermissionDb.CreateEngine();

            if (CommonAction.IsSysAdmin(userId))
            {
                var roles = from role in engine.Esmart_Sys_Roles
                            where role.StartTime <= DateTime.Now && role.EndTime >= DateTime.Now
                            select role.RoleName;

                return roles.ToList();
            }
            else
            {
                var roles = from role in engine.Esmart_Sys_Roles
                            join userRole in engine.Esmart_Sys_User_Roles on role.RoleId equals userRole.RoleId
                            where role.StartTime <= DateTime.Now && role.EndTime >= DateTime.Now && userRole.UserId == userId
                            select role.RoleName;

                return roles.ToList();
            }
        }

        public static List<Esmart_Sys_Users> GetSysUsers(Esmart_Sys_Users condition)
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Sys_Users.Where(u => u.UserName.Contains(condition.UserName)).ToList();
        }

        public static Esmart_Sys_Users GetSingle(Expression<Func<Esmart_Sys_Users, bool>> predicate)
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Sys_Users.Where(predicate).FirstOrDefault();
        }

        readonly static object lockObj = new object();

        /// <summary>
        /// 添加用户，返回用户ID
        /// </summary>
        public static int Add(Esmart_Sys_Users model)
        {
            lock (lockObj)
            {
                return AddUser(model);
            }
        }

        private static int AddUser(Esmart_Sys_Users model)
        {
            var engine = PermissionDb.CreateEngine();

            CheckUser(engine, model);

            model.UserID = (engine.Esmart_Sys_Users.Max(m => (int?)m.UserID) ?? 0) + 1;

            var password = "123456"; //RandomString.Generate(8);

           // new MailService().SendRegisterInfo(model.Email, model.UserID, password);

            model.PassWord = CommonFunction.GetMD5String(password);
            model.Isleave = 0;

            engine.Esmart_Sys_Users.Add(model);

            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = System.DateTime.Now, EventType = "add", OprTbName = "Esmart_Sys_Users", OprUserId = model.CreateId, OptDescription = string.Format("用户：{0}添加了用户信息,用户ID：{1}", model.CreateId, model.UserID), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(model) });
            CommonAction.ClearCache(model.UserID);

            return model.UserID;
        }

        public static bool ResetPwd(int userId,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();

            var user = engine.Esmart_Sys_Users.FirstOrDefault(u => u.UserID == userId);

            if (user == null) throw new TpoBaseException("用户不存在");

            var password = "123456";//RandomString.Generate(8);

            //new MailService().SendPasswordReseted(user.Email, user.UserID, password);

            user.PassWord = CommonFunction.GetMD5String(password);

            engine.SaveChanges();

            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "resetpwd", OprTbName = "Esmart_Sys_Users", OprUserId = optUserId, OptDescription = string.Format("用户：{0}重置了用户{1}密码", optUserId,userId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(user) });
            return true;
        }

        public static bool Update(UpdateUserDto model,int optUserId=0)
        {
            try
            {
                var engine = PermissionDb.CreateEngine();

                CheckUser(engine, model);

                var user = engine.Esmart_Sys_Users.Find(model.UserID);

                if (user == null)
                    throw new TpoBaseException("用户不存在，请刷新后重试");

                Mapper.Map(model, user);
                
                engine.SaveChanges();
                RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "update", OprTbName = "Esmart_Sys_Users", OprUserId = optUserId, OptDescription = string.Format("用户：{0}更新了用户{1}信息", optUserId, model.UserID), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(model) });
            }
            catch (DbEntityValidationException e)
            {
                var sb = new StringBuilder(200);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendFormat("[{0}:{1}] ", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new TpoBaseException(sb.ToString());
            }
            catch (Exception e)
            {
                var message = e.Message;
                if (e.InnerException != null)
                    message += e.InnerException.Message;
                throw new TpoBaseException(message);
            }
            return true;
        }

        public static bool Update2(Esmart_Sys_Users model)
        {
            try
            {
                var engine = PermissionDb.CreateEngine();

                //CheckUser(engine, model);

                var user = engine.Esmart_Sys_Users.Find(model.UserID);

                if (user == null)
                    throw new TpoBaseException("用户不存在，请刷新后重试");

                Mapper.Map(model, user);

                engine.SaveChanges();
                RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = System.DateTime.Now, EventType = "update", OprTbName = "Esmart_Sys_Users", OprUserId = model.CreateId, OptDescription = string.Format("用户：{0}更新了用户{1}信息", model.CreateId, model.UserID), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(model) });
            }
            catch (DbEntityValidationException e)
            {
                var sb = new StringBuilder(200);
                foreach (var eve in e.EntityValidationErrors)
                {
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendFormat("[{0}:{1}] ", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new TpoBaseException(sb.ToString());
            }
            catch (Exception e)
            {
                var message = e.Message;
                if (e.InnerException != null)
                    message += e.InnerException.Message;
                throw new TpoBaseException(message);
            }
            return true;
        }

        /// <summary>
        /// 设置用户的离职状态  1:离职  0：未离职
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool UpdateLeaveStatus(int userId,int optUserId=0)
        {
            //更新BaseManager库中的数据
            StringBuilder sb = new StringBuilder();
            //更新SysManager库中的数据
            var engine = PermissionDb.CreateEngine();
            var user = engine.Esmart_Sys_Users.FirstOrDefault(n => n.UserID == userId);
            if (user != null) user.Isleave = 1;
            sb.Append("用户状态更新：").Append(Newtonsoft.Json.JsonConvert.SerializeObject(user)).Append(";");
            var roles = engine.Esmart_Sys_User_Roles.Where(n => n.UserId == userId).ToArray();
            engine.Esmart_Sys_User_Roles.RemoveRange(roles);
            sb.Append("用户离职后删除其角色：").Append(Newtonsoft.Json.JsonConvert.SerializeObject(roles)).Append(";");
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "update&delete", OprTbName = "Esmart_Teacher_Info,Esmart_Sys_Users,Esmart_Sys_User_Roles", OprUserId = optUserId, OptDescription = string.Format("用户：{0}更新了用户{1}离职状态信息,,角色ID{2}", optUserId, userId,string.Join(",",roles.Select(s=>s.Id))), Remark =sb.ToString() });
            sb.Length = 0;
            return true;
        }

        public static bool ModifyPassword(string userAccount, string password, string newPassword,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            var user = engine.Esmart_Sys_Users.FirstOrDefault(m => m.Isleave != 1 && m.IsDelete != 1 && (m.Email == userAccount || m.Mobile == userAccount));

            if (user == null) throw new TpoBaseException("用户帐号不存在");

            if (user.PassWord != CommonFunction.GetMD5String(password))
                throw new TpoBaseException("用户密码错误");

            if (password == newPassword)
                throw new TpoBaseException("新密码不能与旧密码相同!");

            user.PassWord = CommonFunction.GetMD5String(newPassword);
            engine.SaveChanges();

            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "update", OprTbName = "Esmart_Sys_Users", OprUserId = optUserId, OptDescription = string.Format("用户：{0}修改了用户{1}密码,账号为：{2}", optUserId, user.UserID, userAccount), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(user) });
            return true;
        }

        #region 内部静态帮助类

        private static void CheckUser(PermissionContext dbContext, Esmart_Sys_Users user)
        {
            //说明:用户已经离职，此用户的状态相当于被彻底删除，故其手机号和邮箱可以被重新启用

            //-------------------------------------------------
            // 工号检查
            //-------------------------------------------------

            if (string.IsNullOrWhiteSpace(user.WorkNo))
            {
                throw new TpoBaseException("工号不能为空!");
            }
            else
            {
                user.WorkNo = user.WorkNo.Trim();
                if (dbContext.Esmart_Sys_Users.Any(n => n.UserID != user.UserID && n.WorkNo == user.WorkNo))
                    throw new TpoBaseException("工号已被其它员工占用!");
            }

            //-------------------------------------------------
            // 邮箱检查
            //-------------------------------------------------

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new TpoBaseException("邮箱不能为空!");

            user.Email = user.Email.Trim();

            if (dbContext.Esmart_Sys_Users.Any(n => n.UserID != user.UserID && n.Email == user.Email && n.Isleave != 1))
                throw new TpoBaseException("邮箱已被其它员工占用，如果需要在部门中添加此用户，可以使用调入用户功能!");

            //-------------------------------------------------
            // 手机号码检查
            //-------------------------------------------------

            if (string.IsNullOrWhiteSpace(user.Mobile))
                throw new TpoBaseException("手机号码不能为空!");

            user.Mobile = user.Mobile.Trim();

            if (dbContext.Esmart_Sys_Users.Any(n => n.UserID != user.UserID && n.Mobile == user.Mobile && n.Isleave != 1))
                throw new TpoBaseException("手机号码已被其它员工占用，如果需要在部门中添加此用户，可以使用调入用户功能!");
        }

        private static void CheckUser(PermissionContext dbContext, UpdateUserDto user)
        {
            //说明:用户已经离职，此用户的状态相当于被彻底删除，故其手机号和邮箱可以被重新启用

            //-------------------------------------------------
            // 工号检查
            //-------------------------------------------------

            if (string.IsNullOrWhiteSpace(user.WorkNo))
            {
                throw new TpoBaseException("工号不能为空!");
            }

            user.WorkNo = user.WorkNo.Trim();
            if (dbContext.Esmart_Sys_Users.Any(n => n.UserID != user.UserID && n.WorkNo == user.WorkNo))
                throw new TpoBaseException("工号已被其它员工占用!");

            //-------------------------------------------------
            // 邮箱检查
            //-------------------------------------------------

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new TpoBaseException("邮箱不能为空!");

            user.Email = user.Email.Trim();

            if (dbContext.Esmart_Sys_Users.Any(n => n.UserID != user.UserID && n.Email == user.Email && n.Isleave != 1))
                throw new TpoBaseException("邮箱已被其它员工占用，如果需要在部门中添加此用户，可以使用调入用户功能!");

            //-------------------------------------------------
            // 手机号码检查
            //-------------------------------------------------

            if (string.IsNullOrWhiteSpace(user.Mobile))
                throw new TpoBaseException("手机号码不能为空!");

            user.Mobile = user.Mobile.Trim();

            if (dbContext.Esmart_Sys_Users.Any(n => n.UserID != user.UserID && n.Mobile == user.Mobile && n.Isleave != 1))
                throw new TpoBaseException("手机号码已被其它员工占用，如果需要在部门中添加此用户，可以使用调入用户功能!");
        }

        #endregion
    }
}
