using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Esmart.Framework.Configuration;
using Esmart.Framework.Config;

namespace Esmart.Framework
{
    public  class GlobalConfig
    {
        #region 数据库连接
        public static string LogHelpDB
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["LogHelpDB"].ToString(); }
        }
        public static string TpoEduManagerContext
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["TpoEduManagerContext"].ToString(); }
        }
        public static string TpoBaseManagerContext
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["TpoBaseManagerContext"].ToString(); }
        }
        public static string TpoSysManagerContext
        {
            get { return ConfigurationManager.ConnectionStrings["TpoSysManagerContext"].ToString(); }
        }
        public static string TpoMessageContext
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["TpoMessageContext"].ToString(); }
        }
        public static string SoaCommonDB
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["SoaCommonDB"].ToString(); }
        }
        #endregion


        #region 系统级别配置文件key值

        public static string NotUsePlatForm
        {
            get {
                return CachedConfigContext.Current.SystemConfig.NotUsePlatForm.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["NotUsePlatForm"].ToString();
            }
        }
        public static string NotUserCache
        {
            get {
                return CachedConfigContext.Current.SystemConfig.NotUserCache.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["NotUserCache"].ToString();
            }
        }
        public static string NotUserLog
        {
            get {
                return CachedConfigContext.Current.SystemConfig.NotUserLog.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["NotUserLog"].ToString();
            }
        }
  
        public static string Debug
        {
            get {
                return CachedConfigContext.Current.SystemConfig.Debug.ToString();
              //  return System.Configuration.ConfigurationManager.AppSettings["Debug"].ToString();
            }
        }
        public static string isprofile
        {
            get {
                return CachedConfigContext.Current.SystemConfig.isprofile.ToString();
              //  return System.Configuration.ConfigurationManager.AppSettings["isprofile"].ToString();
            }
        }
        public static string AppID
        {
            get {
                return CachedConfigContext.Current.SystemConfig.AppID.ToString();
                //return System.Configuration.ConfigurationManager.AppSettings["AppID"].ToString();
            }
        }
        /// <summary>
        /// webservice服务地址
        /// </summary>
        public static string SoaDomain
        {
            get {
                return CachedConfigContext.Current.SystemConfig.SoaDomain.ToString();
                //return System.Configuration.ConfigurationManager.AppSettings["SoaDomain"].ToString();
            }
        }
        /// <summary>
        /// 权限地址
        /// </summary>
        public static string AuthorityAddredd
        {
            get {
                return CachedConfigContext.Current.SystemConfig.AuthorityAddredd.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["AuthorityAddredd"].ToString();
            }
        }
        public static string RedisCacheAddress
        {
            get {
                return CachedConfigContext.Current.SystemConfig.RedisCacheAddress.ToString();
              //  return System.Configuration.ConfigurationManager.AppSettings["RedisCacheAddress"].ToString();
            }
        }
        public static string SentinelMaster
        {
            get { return "mymaster"; }
        }
       
        public static string RabbitAddress
        {
            get {
                return CachedConfigContext.Current.SystemConfig.RabbitAddress.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["RabbitAddress"].ToString();
            }
        }
        public static string RabbitUserName
        {
            get {
                return CachedConfigContext.Current.SystemConfig.RabbitUserName.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["RabbitUserName"].ToString();
            }
        }
        public static string RabbitPassWord
        {
            get {
                return CachedConfigContext.Current.SystemConfig.RabbitPassWord.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["RabbitPassWord"].ToString();
            }
        }
        public static string VirtualHost
        {
            get {
                return CachedConfigContext.Current.SystemConfig.VirtualHost.ToString();
                //return System.Configuration.ConfigurationManager.AppSettings["VirtualHost"].ToString();
            }
        }
        public static string smtpHost
        {
            get {
                return CachedConfigContext.Current.SystemConfig.smtpHost.ToString();
                //return System.Configuration.ConfigurationManager.AppSettings["smtpHost"].ToString();
            }
        }
        public static string smptPort
        {
            get {
                return CachedConfigContext.Current.SystemConfig.smptPort.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["smptPort"].ToString();
            }
        }
        public static string smtpCredentialAccount
        {
            get {
                return CachedConfigContext.Current.SystemConfig.smtpCredentialAccount.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["smtpCredentialAccount"].ToString();
            }
        }
        public static string smtpCredentialPassword
        {
            get {
                return CachedConfigContext.Current.SystemConfig.smtpCredentialPassword.ToString();
               // return System.Configuration.ConfigurationManager.AppSettings["smtpCredentialPassword"].ToString();
            }
        }
        public static string smsServiceUrl
        {
            get {
                return CachedConfigContext.Current.SystemConfig.smsServiceUrl.ToString();
                //return System.Configuration.ConfigurationManager.AppSettings["smsServiceUrl"].ToString();
            }
        }
        public static string smsServiceToken
        {
            get {
                return CachedConfigContext.Current.SystemConfig.smsServiceToken.ToString();
                //return System.Configuration.ConfigurationManager.AppSettings["smsServiceToken"].ToString();
            }
        }
        #endregion


        #region 业务级别配置文件key值
     /// <summary>
     /// API认证ID
     /// </summary>
        public static string ClientId
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["clientId"].ToString(); }
        }
        /// <summary>
        ///  API密钥
        /// </summary>
        public static string ClientSecret
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["clientSecret"].ToString(); }
        }
      
        /// <summary>
        /// Token过期时间
        /// </summary>
        public static string TokenExpire
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["TokenExpire"].ToString(); }
        }
        /// <summary>
        ///  Token刷新时间
        /// </summary>
        public static string RefreshExpire
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["RefreshExpire"].ToString(); }
        }
        #endregion


    }
}

       