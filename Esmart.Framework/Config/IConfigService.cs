using System;
using System.Collections.Generic;
using System.Web.Caching;

namespace Esmart.Framework.Config
{
    public interface IConfigService
    {
        string GetConfig(string name);
        void SaveConfig(string name, string content);
        string GetFilePath(string name);
        string GetFileFolder();
    }
}
