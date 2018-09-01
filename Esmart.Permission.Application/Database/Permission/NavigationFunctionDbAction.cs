using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.Data.Permission
{
    public class NavigationFunctionDbAction
    {
        public static List<Esmart_Sys_Navigation_Function> GetListByNavigationId(int navigationId)
        {
            var engine = PermissionDb.CreateEngine();
            var list = engine.Esmart_Sys_Navigation_Function.Where(a => a.NavigationId == navigationId).ToList();
            CommonAction.ClearCache();
            return list;
        }

        public static bool Update(List<Esmart_Sys_Navigation_Function> listModel, int navigationId)
        {
            var engine = PermissionDb.CreateEngine();
            //删除
            var navigationFunction = engine.Esmart_Sys_Navigation_Function.Where(m => m.NavigationId == navigationId);
            if (navigationFunction.Any())
            {
                engine.Esmart_Sys_Navigation_Function.RemoveRange(navigationFunction);
            }
            if (listModel != null && listModel.Any())
            {
                engine.Esmart_Sys_Navigation_Function.AddRange(listModel);
            }
           int reInt= engine.SaveChanges();
           int createId = listModel.First().CreateId;
           RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = createId, CreateTime = System.DateTime.Now, EventType = "update", OprTbName = "Esmart_Sys_Navigation_Function", OprUserId = createId, OptDescription = string.Format("用户：{0}修改了菜单功能关系,其中删除关系表ID：{1},添加关系表ID：{2}",createId,string.Join(",",navigationFunction.Select(s=>s.NavigationId)),string.Join(",",listModel)), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(navigationFunction)+";"+Newtonsoft.Json.JsonConvert.SerializeObject(listModel) });
            CommonAction.ClearCache();
            return reInt > 0;
        }
    }
}
