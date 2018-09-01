using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_LogInfo
    {
        [Column(Order = 0)]
        public int Id { get; set; }
        
        [Column(Order = 1)]
        [StringLength(50)]
        public string ActionId { get; set; }

        [Column(Order = 2)]
        public DateTime CreateTime { get; set; }

        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CreateID { get; set; }

        [StringLength(3000)]
        public string Remark { get; set; }
    }
}
