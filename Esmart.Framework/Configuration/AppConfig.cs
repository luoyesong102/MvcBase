using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Configuration
{
    public class AppConfig
    {

        public AppConfig()
        {
            ModulesItems = new List<string>();
            WatcherItems = new List<WatcherItem>();
            AppSetting = new Dictionary<string, string>();
            NavigationStyles = new List<NavigationAttr>();
            PermissionComponents = new Dictionary<string, PermissionComponent>();
        }

        public static AppConfig Curent
        {
            get;
            internal set;
        }

        public List<string> ModulesItems { get; set; }

        public List<WatcherItem> WatcherItems { get; set; }

        public Dictionary<string, string> AppSetting { get; internal set; }

        public List<NavigationAttr> NavigationStyles { get; internal set; }

        public Dictionary<string, PermissionComponent> PermissionComponents { get; set; }

        /// <summary>
        /// 模板页菜单设置类， 对应TpoooConfig.xml中的Navigations节点内容
        /// </summary>
        public class NavigationAttr
        {
            public int Order { get; set; }

            public string UlAttr { get; set; }

            public string LiAttr { get; set; }

            public string LinkAttr { get; set; }

            public string IconHtml { get; set; }

            public string IconPosition { get; set; }

            public string Generate(string lis, bool main = false)
            {
                return string.IsNullOrWhiteSpace(lis) ? "" : string.Format("<ul id='tpoCrmNavgation' {0}>{1}</ul>", UlAttr, lis);
            }
        }


        /// <summary>
        /// 权限组成类， 对应TpoooConfig.xml中的PermissionMapping节点的内容
        /// </summary>
        public class PermissionComponent
        {
            public string Navigation { get; set; }

            public string Function { get; set; }
        }
    }
}
