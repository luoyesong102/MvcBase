using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.SOACommonDB
{
    [Table("AppRequest")]
    public partial class AppRequest
    {
        public int AppID { get; set; }

        public int CanUserAppID { get; set; }

        [Required]
        [StringLength(50)]
        public string CreateID { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }

        public int ID { get; set; }
    }
}
