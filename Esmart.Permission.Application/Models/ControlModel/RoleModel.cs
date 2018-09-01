using System;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class RoleModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 开始有效期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束有效期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 角色备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int? CreatorId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        public bool IsBuiltin { get; set; }
    }
}
