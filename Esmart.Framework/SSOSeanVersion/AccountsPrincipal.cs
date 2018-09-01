
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
namespace Esmart.Framework.SSOSeanVersion
{
    public class AccountsPrincipal : IPrincipal
    {
        //private LTP.Accounts.Data.User dataUser;
        IUser dataUser;
        protected IIdentity identity;
        protected ArrayList permissionList;
        protected ArrayList permissionListid;
        protected ArrayList roleList;

        /// <summary>
        /// 通过userid获取权限列表，角色列表
        /// </summary>
        /// <param name="userID"></param>
        public AccountsPrincipal(int userID)
        {
            //this.dataUser = new LTP.Accounts.Data.User();
            this.identity = new SiteIdentity(userID);
            //this.permissionList = this.dataUser.GetEffectivePermissionList(userID);
            //this.permissionListid = this.dataUser.GetEffectivePermissionListID(userID);
            //this.roleList = this.dataUser.GetUserRoles(userID);
        }

        public AccountsPrincipal(string userName)
        {
            //this.dataUser = new LTP.Accounts.Data.User();
            this.identity = new SiteIdentity(userName);
            //this.permissionList = this.dataUser.GetEffectivePermissionList(((SiteIdentity)this.identity).UserID);
            //this.permissionListid = this.dataUser.GetEffectivePermissionListID(((SiteIdentity)this.identity).UserID);
            //this.roleList = this.dataUser.GetUserRoles(((SiteIdentity)this.identity).UserID);
        }

        public static byte[] EncryptPassword(string password)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(password);
            SHA1 sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(bytes);
        }

        public bool HasPermission(string permission)
        {
            return this.permissionList.Contains(permission);
        }

        public bool HasPermissionID(int permissionid)
        {
            return this.permissionListid.Contains(permissionid);
        }

        public bool IsInRole(string role)
        {
            return this.roleList.Contains(role);
        }

        public static AccountsPrincipal ValidateLogin(string userName, string password)
        {
            byte[] encPassword = EncryptPassword(password);
            // LTP.Accounts.Data.User user = new LTP.Accounts.Data.User();

            //通过数据库验证用户
            // int userID = user.ValidateLogin(userName, encPassword);
            int userID = 1;
            if (userID > 0)
            {
                return new AccountsPrincipal(userID);
            }
            return null;
        }

        public IIdentity Identity
        {
            get
            {
                return this.identity;
            }
            set
            {
                this.identity = value;
            }
        }

        public ArrayList Permissions
        {
            get
            {
                return this.permissionList;
            }
        }

        public ArrayList PermissionsID
        {
            get
            {
                return this.permissionListid;
            }
        }

        public ArrayList Roles
        {
            get
            {
                return this.roleList;
            }
        }
    }
}



