using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Config
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    [Serializable]
    public class SystemConfig : ConfigFileBase
    {
        public SystemConfig()
        {
        }

        #region 序列化属性
        public string RedisCacheAddress { get; set; }
        public string AppID { get; set; }
        public string NotUsePlatForm { get; set; }
        public string NotUserCache { get; set; }
        public string NotUserLog { get; set; }
        public string Debug { get; set; }
        public string isprofile { get; set; }
        public string SoaDomain { get; set; }
        public string AuthorityAddredd { get; set; }
        
        public string RabbitAddress { get; set; }
        public string RabbitUserName { get; set; }
        public string RabbitPassWord { get; set; }
        public string VirtualHost { get; set; }
        public string smtpHost { get; set; }
        public string smptPort { get; set; }
        public string smtpCredentialAccount { get; set; }
        public string smtpCredentialPassword { get; set; }
        public string smsServiceUrl { get; set; }
        public string smsServiceToken { get; set; }
     
        #endregion
    }
}
