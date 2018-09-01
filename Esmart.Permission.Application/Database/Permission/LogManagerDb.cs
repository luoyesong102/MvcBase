using System;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.Data
{
    public class LogManagerDb
    {
        public static void Log(int createId, DateTime createTime, string remark)
        {
            var engine = PermissionDb.CreateEngine();
            var model = new Esmart_Sys_LogInfo() { CreateID = createId, CreateTime = createTime, Remark = remark };
            engine.Esmart_Sys_LogInfo.Add(model);
            engine.SaveChanges();
        }

        public static void Log(int createId, DateTime createTime, string remark, string actionId)
        {
            var engine = PermissionDb.CreateEngine();
            var model = new Esmart_Sys_LogInfo()
            {
                CreateID = createId,
                CreateTime = createTime,
                Remark = remark,
                ActionId = actionId
            };
            engine.Esmart_Sys_LogInfo.Add(model);
            engine.SaveChanges();
        }
    }
}
