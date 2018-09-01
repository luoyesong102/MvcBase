using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.Data
{
    public class RightLogDb
    {
        public static void AddLog(Esmart_Right_Log log)
        {
            try
            {
                if (log != null)
                {
                    var engine = PermissionDb.CreateEngine();
                    engine.Esmart_Right_Log.Add(log);
                    engine.SaveChanges();
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public static List<Esmart_Right_Log> GetRightLogs(HashSet<string> hash)
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Right_Log.ToList();
        }
    }
}
