using System;
using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class RolePermissionsRequestModel
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        public int AppId { get; set; }
        /// <summary>
        /// 菜单Id
        /// </summary>

        public ICollection<int> NavigationsCollection { get; set; }
        /// <summary>
        /// 功能Id--菜单Id
        /// </summary>

        public ICollection<KeyValueModel<int, int>> FunctionsCollection { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>

        public int? CreatorId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>

        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>

        public int? UpdaterId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>

        public DateTime? UpdateTime { get; set; }
    }
}
