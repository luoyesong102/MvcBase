using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Functions
    {
        public int AppId { get; set; }

        [Key]
       
        public int FunctionId { get; set; }

        [StringLength(50)]
        public string FunctionName { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        [StringLength(50)]
        public string FunctionKey { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }

        public int IsDelete { get; set; }
    }
}
