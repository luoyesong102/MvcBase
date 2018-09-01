using System;
using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.ControlModel
{
    [Serializable]
    public class DepartmentResponse
    {
        /// <summary>
        /// 部门Id
        /// </summary>
        public int DeparentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父级Id
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int? IsDelete { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CreateId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 子级部门列表
        /// </summary>
        public List<DepartmentResponse> Children { get; set; }

        public List<DepartmentUserResponse> Users { get; set; }
    }
}
