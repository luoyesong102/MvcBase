using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Navigations
    {
        public int AppId { get; set; }

        [PrimarykeyAttrbute]
        [Key]
       
        public int NavigationId { get; set; }

        public int SortNo { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(1024)]
        public string Url { get; set; }

        public int ParentID { get; set; }

        [StringLength(500)]
        public string Iconurl { get; set; }

        [StringLength(50)]
        public string InClassName { get; set; }

        [StringLength(50)]
        public string OutClassName { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
