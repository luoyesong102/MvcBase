using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Serialization;
namespace Esmart.Framework.DB
{
    [Serializable]
    public class QueryParameter
    {        
        public string Name{get;set;}
      
        public object Value  {get;set;}
        
        public DbType Type { get; set; }
    }
}
