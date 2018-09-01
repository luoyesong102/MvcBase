using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class LoginUserInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名字
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 菜单信息
        /// </summary>
        public List<MenuResponse> Menus { get; set; }

        //public string UserIP { get; set; }

        //public DateTime ExpiredAt { get; set; }
    }
}
