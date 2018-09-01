using System.Collections.Generic;

namespace Esmart.Permission.Application
{
    public class CacheKey
    {
        //---------------------------------------------------
        // 系统内置缓存key
        //---------------------------------------------------

        public const string Authorize_UserRole = "Authorize_UserRole_";
        public const string Authorize_DepartmentDic = "Authorize_DepartmentDic";

        //---------------------------------------------------
        // 以下为ERP缓存Key
        //---------------------------------------------------

        /// <summary>
        /// 用户基础信息缓存key
        /// </summary>
        public const string BaseUserInfoKey = "Esmart_Base_UserInfo_Key_2015";

        /// <summary>
        /// 老师缓存基础信息key
        /// </summary>
        public const string TearcherInfoKey = "Esmart_Tearcher_Info_Key_2015";

        /// <summary>
        /// 用户信息缓存Key，缓存用户ID在{0}和{1}之间的数据
        /// </summary>
        public const string UserInfoCacheKey = "Esmart_User_Info_{0}_{1}";

        /// <summary>
        /// 学生下拉列表
        /// </summary>
        public const string UserDropDownKey = "User_DropDown_Key_2015";

        public static KeyValuePair<int, int> GetKeyValue(int id)
        {
            if (id < 100)
            {
                return new KeyValuePair<int, int>(0, 99);
            }
            int value = id - id % 100;

            return new KeyValuePair<int, int>(value, value + 99);
        }

        public static string CreateKey(string key, KeyValuePair<int, int> keyvalue)
        {
            return string.Format("{0}_{1}_{2}", key, keyvalue.Key, keyvalue.Value);
        }
    }
}
