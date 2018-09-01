using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class MenuRequest
    {

        /// <summary>
        /// 菜单Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<MenuRequest> Child { get; set; }
        /// <summary>
        /// 功能列表
        /// </summary>

        public List<int> Fuctions { get; set; }

    }
}
