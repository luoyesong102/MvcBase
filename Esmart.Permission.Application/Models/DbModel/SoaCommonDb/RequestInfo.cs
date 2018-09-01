using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.SOACommonDB
{
    [Table("RequestInfo")]
    public partial class RequestInfo
    {
        [Key]
        [StringLength(100)]
        public string RequestType { get; set; }

        public int AppID { get; set; }

        [StringLength(1024)]
        public string RequestUrl { get; set; }

        public int TimeOut { get; set; }

        [Required]
        [StringLength(45)]
        public string UserID { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }

        [StringLength(100)]
        public string GetCacheKey { get; set; }

        [StringLength(100)]
        public string RemoveCacheKey { get; set; }

        public string RequestJson { get; set; }

        public string Remark { get; set; }
    }
}
