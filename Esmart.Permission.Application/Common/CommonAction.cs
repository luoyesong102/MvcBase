using System;
using System.Configuration;
using System.Diagnostics;
using Esmart.Permission.Application.Constants;
using Esmart.Framework.Caching;

namespace Esmart.Permission.Application
{
    public static class CommonAction
    {
        /// <summary>
        /// 是否是系统管理员
        /// </summary>.
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool IsSysAdmin(int userId)
        {
            return ConfigurationManager.AppSettings["SysAdminUserId"] == userId.ToString();
        }
        public static bool IsChecked()
        {
            TimeSpan ts = DateTime.Now - ServiceUrl.dates;
            int nDays = ts.Days;
          return  nDays > 0 ? true : false;
        }
        public static void ClearCache(int userId = 0)
        {
            try
            {
                CacheManager.CreateCache().FlushAll();

                if (userId > 0)
                {
                    var keyValue = CacheKey.GetKeyValue(userId);
                    var key = CacheKey.CreateKey(CacheKey.BaseUserInfoKey, keyValue);
                    CacheManager.CreateRedisCache().Remove(key);

                    key = CacheKey.CreateKey(CacheKey.TearcherInfoKey, keyValue);
                    CacheManager.CreateRedisCache().Remove(key);

                    CacheManager.CreateRedisCache().Remove(CacheKey.UserDropDownKey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
       
    }
}
