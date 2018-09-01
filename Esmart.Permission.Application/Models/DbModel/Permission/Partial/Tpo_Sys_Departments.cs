using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Departments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DeparentId { get; set; }

        public int SortNo { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int ParentId { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }

        public int IsDelete { get; set; }
    }
}
