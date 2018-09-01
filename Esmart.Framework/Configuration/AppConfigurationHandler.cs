using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Esmart.Framework.Configuration
{
    public class AppConfigurationHandler : System.Configuration.IConfigurationSectionHandler
    {

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            var config = new AppConfig();

            foreach (XmlNode node in section.ChildNodes)
            {
                switch (node.Name)
                {
                    case "ModulesDLL":
                        #region ModulesDLL

                        foreach (XmlNode item in node.ChildNodes)
                        {
                            if (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Equals("item") &&
                                item.Attributes != null)
                            {
                                var nameAttribute = item.Attributes["Name"];
                                if(nameAttribute == null) continue;
                                config.ModulesItems.Add(nameAttribute.Value);
                            }
                        }

                        #endregion
                        break;
                    case "WatchItems":
                        #region WatchItems
                        foreach (XmlNode wItem in node.ChildNodes)
                        {
                            if (!string.IsNullOrEmpty(wItem.Name) && wItem.Name.ToLower() == "item")
                            {
                                WatcherItem item = new WatcherItem();
                                item.Content = wItem.Attributes["Content"].Value;
                                WatcherType tmpType = WatcherType.None;
                                Enum.TryParse<WatcherType>(wItem.Attributes["Type"].Value, out tmpType);
                                item.Type = tmpType;
                                item.Filter = wItem.Attributes["Filter"].Value;
                                config.WatcherItems.Add(item);
                            }
                        }
                        #endregion
                        break;
                    case "AppSetting":
                        #region AppSetting

                        foreach (XmlNode item in node.ChildNodes)
                        {
                            if (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Equals("add") && item.Attributes != null)
                            {
                                var keyAttribute = item.Attributes["Key"];
                                var valueAttribute = item.Attributes["Value"];
                                if(keyAttribute == null || valueAttribute == null) continue;
                                config.AppSetting.Add(keyAttribute.Value, valueAttribute.Value);
                            }
                        }

                        #endregion
                        break;
                    case "Navigations":
                        #region Navigations

                        foreach (XmlNode item in node.ChildNodes)
                        {
                            if (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Equals("add") && item.Attributes != null)
                            {
                                var orderAttribute = item.Attributes["Order"];
                                var liAttribute = item.Attributes["ParentLiAttr"];
                                var ulAttribute = item.Attributes["UlAttr"];
                                var linkAttr = item.Attributes["LinkAttr"];
                                var linkIcon = item.Attributes["LinkIcon"];
                                var linkIconPosition = item.Attributes["LinkIconPosition"];
                                if (orderAttribute == null) continue;
                                config.NavigationStyles.Add(new AppConfig.NavigationAttr
                                {
                                    Order = Convert.ToInt32(orderAttribute.Value),
                                    LiAttr = liAttribute == null ? "" : liAttribute.Value,
                                    UlAttr = ulAttribute == null ? "" : ulAttribute.Value,
                                    IconHtml = linkIcon == null ? "" : linkIcon.Value,
                                    IconPosition = linkIconPosition == null ? "behind" : linkIconPosition.Value,
                                    LinkAttr = linkAttr == null ? "" : linkAttr.Value
                                });
                            }
                        }

                        #endregion
                        break;
                    case "PermissionMapping":
                        #region PermissionMapping

                        foreach (XmlNode item in node.ChildNodes)
                        {
                            if (!string.IsNullOrEmpty(item.Name) && item.Name.ToLower().Equals("add") && item.Attributes != null)
                            {
                                var nameAttribute = item.Attributes["Name"];
                                var navigationAttribute = item.Attributes["Navigation"];
                                var functionAttribute = item.Attributes["Function"];
                                config.PermissionComponents.Add(nameAttribute.Value, new AppConfig.PermissionComponent
                                {
                                    Navigation = navigationAttribute.Value,
                                    Function = functionAttribute.Value
                                });
                            }
                        }

                        #endregion
                        break;
                }
            }

            AppConfig.Curent = config;
            return config;
        }
    }
}
