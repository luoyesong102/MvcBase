using System;
using System.Collections.Generic;
using Esmart.Permission.Application;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web
{
    public class UserService
    {
        readonly IUser _userManager;

        public UserService()
        {
            _userManager = new UserManager();
        }

        public Esmart_Sys_Users GetUser(int userId)
        {
            try
            {
                return _userManager.GetSingleUser(userId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool UpdateUser(Esmart_Sys_Users user)
        {
            try
            {
                return _userManager.UpdateUser2(user);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool DeleteUser(int userId, int departMentId)
        {
            try
            {
                return _userManager.DeleteUser(userId, departMentId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public int CreateUserWithDepartmentId(Esmart_Sys_Users user, int departMentId)
        {
            try
            {
                return _userManager.CreateUserWithDepartmentId(user, departMentId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public List<DepartmentUserResponse> GetUserOutDepartment(int departmentId, int loginUserId)
        {
            try
            {
                return _userManager.GetUserOutDepartment(departmentId, loginUserId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        public bool ResetUserPwd(int userId)
        {
            try
            {
                return _userManager.ResetUserPwd(userId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        /// <summary>
        /// 用户离职
        /// </summary>
        public bool UpdateLeaveStatus(int userId)
        {
            try
            {
                return _userManager.UpdateLeaveStatus(userId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public List<Esmart_Sys_Users> GetUsers(Esmart_Sys_Users condition)
        {
            try
            {
                return _userManager.GetUsers(condition);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool ModifyPassword(string userAccount, string password, string newPassword)
        {
            try
            {
                return _userManager.ModifyPassword(userAccount, password, newPassword);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool ChangeDepartmentOfUser(List<int> userIds, int newDepartmentId, int createId)
        {
            try
            {
                return _userManager.ChangeDepartmentOfUser(userIds, newDepartmentId, createId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }
    }
}
