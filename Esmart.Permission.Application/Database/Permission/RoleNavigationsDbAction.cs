using System.Linq;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.Data
{
    public class RoleNavigationsDbAction
    {
        /// <summary>
        /// 删除当前角色Id对应的数据
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public static bool Del(int roleId,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            var entitys = engine.Esmart_Sys_Role_Navigations.Where(a => a.RoleId == roleId).ToList();
            engine.Esmart_Sys_Role_Navigations.RemoveRange(entitys);
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Role_Navigations", OprUserId = optUserId, OptDescription = string.Format("用户：{0}删除了角色菜单关系,角色ID：{1}", optUserId,roleId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(entitys) });
            CommonAction.ClearCache();
            return true;
        }
    }
}
