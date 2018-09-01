using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Roles
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoleId { get; set; }

        [StringLength(50)]
        public string RoleName { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 有效开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 有效结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        public bool IsBuiltin { get; set; }
    }
}
