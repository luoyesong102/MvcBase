using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Data
{
    public class FunctionDbAction
    {
        public static List<Esmart_Sys_Functions> GetAllFunctions(int appId)
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Sys_Functions.Where(a => a.AppId == appId && a.IsDelete == 0).ToList();
        }

        public static List<Esmart_Sys_Navigation_Function> GetMenusFunctions(List<int> menuIds)
        {
            var engine = PermissionDb.CreateEngine();
            var list = engine.Esmart_Sys_Navigation_Function.Where(a => menuIds.Contains(a.NavigationId));
            return list.ToList();
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static SoaDataPageResponse<Esmart_Sys_Functions> GetList(SoaDataPage<FunctionQueryModelRequest> model)
        {
            SoaDataPageResponse<Esmart_Sys_Functions> response = new SoaDataPageResponse<Esmart_Sys_Functions>();

            var engine = PermissionDb.CreateEngine();
            var list = engine.Esmart_Sys_Functions.Where(m => m.IsDelete == 0);
            if (model.Where.AppId > 0)
            {
                list = list.Where(m => m.AppId == model.Where.AppId);
            }
            if (!string.IsNullOrEmpty(model.Where.Name))
            {
                list = list.Where(m => (m.FunctionKey.Contains(model.Where.Name) || m.FunctionName.Contains(model.Where.Name)));
            }
            response.Count = list.Count();

            //分页
            var listFunctions = list.OrderByDescending(m => m.CreateTime).Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();

            response.Body = listFunctions;

            return response;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Add(Esmart_Sys_Functions model)
        {
            var engine = PermissionDb.CreateEngine();
            //appid相同key不能重复
            if (engine.Esmart_Sys_Functions.Any(m => m.AppId == model.AppId && m.FunctionKey == model.FunctionKey))
            {
                throw new TpoBaseException("功能key已存在,请重新输入");
            }

            model.FunctionId = 1;
            if (engine.Esmart_Sys_Functions.Any())
            {
                model.FunctionId = engine.Esmart_Sys_Functions.Max(m => m.FunctionId) + 1;
            }

            engine.Esmart_Sys_Functions.Add(model);
            var count = engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = System.DateTime.Now, EventType = "add", OprTbName = "Esmart_Sys_Functions", OprUserId = model.CreateId, OptDescription =string.Format("用户：{0}添加了功能,ID：{1}",model.CreateId,model.FunctionId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(model) });
            CommonAction.ClearCache();
            return count > 0;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        public static bool Del(int functionId,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            var functions = engine.Esmart_Sys_Functions.Find(functionId);
            if (functions == null) return false;
            functions.IsDelete = 1;
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Functions", OprUserId =optUserId, OptDescription = string.Format("用户：{0}删除了功能,ID：{1}", optUserId,functionId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(functions) });
            CommonAction.ClearCache();
            return true;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool Update(Esmart_Sys_Functions model)
        {
            var engine = PermissionDb.CreateEngine();
            if (engine.Esmart_Sys_Functions.Any(m => m.FunctionId != model.FunctionId && m.FunctionKey == model.FunctionKey && m.AppId == model.AppId))
            {
                throw new TpoBaseException("功能key已存在,请重新输入");
            }

            var functions = engine.Esmart_Sys_Functions.Find(model.FunctionId);
            if (functions != null)
            {
                functions.AppId = model.AppId;
                functions.CreateId = model.CreateId;
                functions.FunctionKey = model.FunctionKey;
                functions.FunctionName = model.FunctionName;
                functions.Remark = model.Remark;
                engine.SaveChanges();
                RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = System.DateTime.Now, EventType = "update", OprTbName = "Esmart_Sys_Functions", OprUserId = model.CreateId, OptDescription = string.Format("用户：{0}修改了功能,ID：{1}", model.CreateId, model.FunctionId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(functions) });
                return true;
            }

            CommonAction.ClearCache();
            return false;
        }

        public static Esmart_Sys_Functions GetFunctionById(int functionId)
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Sys_Functions.Find(functionId);
        }
    }
}
