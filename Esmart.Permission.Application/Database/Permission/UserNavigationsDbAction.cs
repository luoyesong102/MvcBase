using System.Linq;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application.Data
{
    public class UserNavigationsDbAction
    {
        /// <summary>
        /// 删除当前角色Id对应的数据
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public static bool Del(int userId, int optUserId)
        {
            var engine = PermissionDb.CreateEngine();
            var entitys = engine.Esmart_Sys_User_Navigations.Where(a => a.UserId == userId).ToList();
            engine.Esmart_Sys_User_Navigations.RemoveRange(entitys);
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_User_Navigations", OprUserId = optUserId, OptDescription = string.Format("用户：{0}删除了用户菜单关系,用户ID：{1}", optUserId, userId), Remark = Newtonsoft.Json.JsonConvert.SerializeObject(entitys) });
            CommonAction.ClearCache();
            return true;
        }
    }
}
