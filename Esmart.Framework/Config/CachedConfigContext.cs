using System;
using Esmart.Framework.Utility;
using Esmart.Framework.Caching;
using System.Web.Caching;


namespace Esmart.Framework.Config
{
    public class CachedConfigContext : ConfigContext
    {
        /// <summary>
        /// 重写基类的取配置，加入缓存机制
        /// </summary>
        public override T Get<T>(string index = null)
        {
            var fileName = this.GetConfigFileName<T>(index);
            var key = "ConfigFile_" +fileName;
            var content = Esmart.Framework.Caching.Caching.Get(key);
            if (content != null)
                return (T)content;

            var value = base.Get<T>(index);
            Esmart.Framework.Caching.Caching.Set(key, value, new CacheDependency(ConfigService.GetFilePath(fileName)));
            return value;
        }

        public static CachedConfigContext Current = new CachedConfigContext();

        public DaoConfig DaoConfig
        {
            get
            {
                return this.Get<DaoConfig>();
            }
        }

        public CacheConfig CacheConfig
        {
            get
            {
                return this.Get<CacheConfig>();
            }
        }

        public AdminMenuConfig AdminMenuConfig
        {
            get
            {
                return this.Get<AdminMenuConfig>();
            }
        }

        public SettingConfig SettingConfig
        {
            get
            {
                return this.Get<SettingConfig>();
            }
        }

        public SystemConfig SystemConfig
        {
            get
            {
                return this.Get<SystemConfig>();
            }
        }

        public UploadConfig UploadConfig
        {
            get
            {
                return this.Get<UploadConfig>();
            }
        }
    }
}
