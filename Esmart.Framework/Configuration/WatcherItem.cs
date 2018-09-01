using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Configuration
{
    public enum WatcherType
    {
         None,
         File,
         DataBase
    }

    public class WatcherItem
    {
        public WatcherType Type { get; set; }

        public string Content { get; set; }

        public string Filter { get; set; }
    }
}
