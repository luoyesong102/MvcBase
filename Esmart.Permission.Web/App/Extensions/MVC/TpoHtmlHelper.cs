using System;
using System.Web.Mvc;

namespace Esmart.Permission.Web.MVC
{
    public class TpoHtmlHelper
    {
        public static object GetModelStateValue( ViewContext viewContext, string key, Type destinationType )
        {
            ModelState modelState;
            if( viewContext.ViewData.ModelState.TryGetValue( key, out modelState ) )
            {
                if( modelState.Value != null )
                {
                    return modelState.Value.ConvertTo( destinationType, null /* culture */);
                }
            }
            return null;
        }
    }
}
