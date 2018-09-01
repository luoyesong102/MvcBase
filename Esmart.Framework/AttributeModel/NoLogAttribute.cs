using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Esmart.Framework.Logging
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class NoLogAttribute : Attribute
    { 
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true)]
    public class NoTpoAuthorizeAttribute : Attribute
    {
    } 
}