using System;
using System.Linq;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Services
{
    public class AuthenticationService
    {
        public UserLiteDto GetUser(string loginName, string password)
        {
         
            var pwdMD5 = CommonFunction.GetMD5String(password);
            var engine = PermissionDb.CreateEngine();
            var dbUser = engine.Esmart_Sys_Users.FirstOrDefault(
                    a => (a.Mobile == loginName || a.Email == loginName) && a.PassWord == pwdMD5);

            if (dbUser == null)
            {
                throw new TpoBaseException("用户登录失败，请重新登录");
            }

            if (dbUser.Isleave == 1)
            {
                throw new TpoBaseException("用户已经离职，不能再登录系统");
            }

            return new UserLiteDto
            {
                UserID = dbUser.UserID,
                Email = dbUser.Email,
                TrueName = dbUser.TrueName,
                Ename = dbUser.Ename,
                Sex = dbUser.Sex,
                WorkNo = dbUser.WorkNo
            };
        }
    }
}
