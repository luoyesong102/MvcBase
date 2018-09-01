using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    /// <summary>
    /// 公共常量信息
    /// </summary>
    public class ConstantDefine
    {
        /// <summary>
        /// 每次写入多少数据
        /// </summary>
        public const string BISelectTop = "Framework_LogHelp_BISelectTop";

        public const string Debug = "Framework_LogHelp_Debug";

        public const string Info = "Framework_LogHelp_Info";

        public const string Waring = "Framework_LogHelp_Waring";

        public const string Error = "Framework_LogHelp_Error";

        /// <summary>
        /// 记录信息
        /// </summary>
        public const string Trace = "Trace";
     

        private static bool  _notUsePlatForm;

        static ConstantDefine()
        {
            bool notUsePlatForm = false;

            bool.TryParse(GlobalConfig.NotUsePlatForm, out notUsePlatForm);

            _notUsePlatForm = notUsePlatForm;

            _redisCacheAddress = GlobalConfig.RedisCacheAddress;

            _soaDomain = GlobalConfig.SoaDomain;

            _appId = Convert.ToInt32(GlobalConfig.AppID);



            bool.TryParse(GlobalConfig.NotUserLog, out _isNotUserLog);


            bool.TryParse(GlobalConfig.NotUserCache, out _notUserCache);

            _notUserCache = _notUserCache && !string.IsNullOrEmpty(ConstantDefine.RedisCacheAddress);



            bool.TryParse(GlobalConfig.Debug, out _debug);
            bool.TryParse(GlobalConfig.isprofile, out _isprofile);


        }


        /// <summary>
        /// 设置缓存不可用
        /// </summary>
        internal static void SetCacheUnUse()
        {
            _notUserCache = true;
        }

        internal static void SetNotUserLog()
        {
            _isNotUserLog = true;

        }
        /// <summary>
        /// 是否使用公共平台（true不使用，false使用）
        /// </summary>
        public static bool NotUsePlatForm
        {
            get
            {
                return _notUsePlatForm;
            }
        }


        private static string _redisCacheAddress;
        /// <summary>
        /// Redis地址
        /// </summary>
        public static string RedisCacheAddress
        {
            get
            {
                return _redisCacheAddress;
            }
        }


        private static string _soaDomain;
        /// <summary>
        /// soa服务域名
        /// </summary>
        public static string SoaDomain
        {
            get
            {
                return _soaDomain;
            }
        }


        private static int _appId;
        /// <summary>
        /// 运用程序唯一ID
        /// </summary>
        public static int AppID
        {
            get { return _appId; }

        }

        private static bool _isNotUserLog;
        /// <summary>
        /// 是否记录日记
        /// </summary>
        public static bool NotUserLog
        {
            get
            {
                return _isNotUserLog;
            }
        }
        private static bool _isprofile;
        /// <summary>
        /// 是否监控
        /// </summary>
        public static bool isprofile
        {
            get
            {
                return _isprofile;
            }
        }
        private static bool _notUserCache;
        /// <summary>
        /// 是否使用cache
        /// </summary>
        public static bool NotUserCache
        {
            get { return _notUserCache; }
        }

        private static bool _debug;
        public static  bool SoaDebug
        {
            get
            {
                return _debug;
            }
        }
    }
}