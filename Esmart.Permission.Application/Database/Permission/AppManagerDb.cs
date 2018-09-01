using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Models.SOACommonDB;

namespace Esmart.Permission.Application.Data
{
    public class AppManagerDb
    {
        public static List<AppInfo> GetAppList()
        {
            return SoaCommonDB.CreateEngine().AppInfo.Where(a => a.IsDelete == 0).ToList();
        }
    }
}
