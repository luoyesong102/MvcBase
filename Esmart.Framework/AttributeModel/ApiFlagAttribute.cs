using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Esmart.Framework.Model
{
    /// <summary>
    /// 用于跨模块调用Service
    /// </summary>
    public class ApiFlagAttribute:Attribute
    {
        private string _name;
        private string _areaName;
        public ApiFlagAttribute(string name)
        {            
            _name = name;
        }         

        /// <summary>
        /// 方法接口别名
        /// </summary>
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
