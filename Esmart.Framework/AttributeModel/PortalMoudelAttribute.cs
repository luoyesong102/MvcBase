using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    /// <summary>
    /// 标记为业务逻辑模块，用于Autofac定制型依赖注入
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PortalMoudelAttribute:Attribute
    {
        public string _name;
        public PortalMoudelAttribute(string name)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
    }
}
