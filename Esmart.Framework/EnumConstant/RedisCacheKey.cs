using System.Collections.Generic;
namespace Esmart.Framework
{
    public class RedisCacheKey
    {
        /// <summary>
        /// 用户基础信息缓存key
        /// </summary>
        public const string BaseUserInfoKey = "Tpo_Base_UserInfo_Key_2015";


        /// <summary>
        /// 老师缓存基础信息key
        /// </summary>
        public const string TearcherInfoKey = "Tpo_Tearcher_Info_Key_2015";


        /// <summary>
        /// 教师下拉列表
        /// </summary>
        public const string TearchDropDownKey = "Tearch_DropDown_Key_2015";


        /// <summary>
        /// 用户下拉列表
        /// </summary>
        public const string UserDropDownKey = "User_DropDown_Key_2015";


        /// <summary>
        /// 基础字典缓存key
        /// </summary>
        public const string BaseDictionaryKey = "Tpo_Base_Dictionary_Key_2015_{0}";


        /// <summary>
        /// 产品缓存key
        /// </summary>
        public const string ProductInfoKey = "Tpo_Product_Info_Key_2015";

        /// <summary>
        /// 学生订单缓存key
        /// </summary>
        public const string StudentOrderKey = "Tpo_Student_Order_Key_2015";

        /// <summary>
        /// 学生基础信息缓存
        /// </summary>
        public const string StudentInfoKey = "Tpo_Student_Info_Key_2015_{0}";


        /// <summary>
        /// 节假日基础信息
        /// </summary>
        public const string HolidayInfoKey = "Tpo_HolidayInfoKey_Key_2015";

        /// <summary>
        /// 字典配置缓存Key
        /// </summary>
        public const string DictionaryConfigurationKey = "Tpo_Dictionary_Configuration_Key_2015_Category";

        /// <summary>
        /// 城市信息缓存Key
        /// </summary>
        public const string CityInfoKey = "Tpo_Base_City_Key_2015";

        /// <summary>
        /// 用户信息缓存Key，缓存用户ID在{0}和{1}之间的数据
        /// </summary>
        public const string UserInfoCacheKey = "Tpo_User_Info_{0}_{1}";



        public static KeyValuePair<int, int> GetKeyValue(int id)
        {
            if (id < 100)
            {
                return new KeyValuePair<int, int>(0, 99);
            }
            int value = id - id % 100;

            return new KeyValuePair<int, int>(value, value + 99);
        }

        public static string CreateKey(string  key,KeyValuePair<int, int> keyvalue)
        {
            return string.Format("{0}_{1}_{2}", key, keyvalue.Key, keyvalue.Value);
        }
    }
}
