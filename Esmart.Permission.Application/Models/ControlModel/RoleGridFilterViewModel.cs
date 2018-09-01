namespace Esmart.Permission.Application.Models.ControlModel
{
   public class RoleGridFilterViewModel
    {
        /// <summary>
        /// 角色名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 觉得创建人Id
        /// </summary>
        public int? CreateId { get; set; }


        /// <summary>
        /// 登录用户Id
        /// </summary>
        public int LogInUserId { get; set; }
    }
}
