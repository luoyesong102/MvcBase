using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esmart.Permission.Web.Models.Common
{
    public class PermissionCons
    {
        public const string common_add = "common_add";
        public const string common_update = "common_update";
        public const string common_delete = "common_delete";

        //------部门管理------
        public const string permission_department_add = "permission_department_add";
        public const string permission_department_update = "permission_department_update";
        public const string permission_department_delete = "permission_department_delete";

        //------用户管理------
        public const string permission_user_add = "permission_user_add";
        public const string permission_user_update = "permission_user_update";
        public const string permission_user_delete = "permission_user_delete";
        public const string permission_user_leave = "permission_user_leave";
        public const string permission_user_password_reset = "permission_user_password_reset";

        //------角色分配------
        public const string permission_role_assign = "permission_role_assign";
    }
}