using System;
using System.ComponentModel.DataAnnotations;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Deparent_Role
    {
        public int DeparentId { get; set; }

        public int RoleId { get; set; }

        [StringLength(50)]
        public string Remark { get; set; }

        public int Id { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
