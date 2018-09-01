using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.SOACommonDB
{
    [Table("AppInfo")]
    public partial class AppInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AppID { get; set; }

        [Required]
        [StringLength(100)]
        public string AppName { get; set; }

        [Required]
        [StringLength(50)]
        public string CreateID { get; set; }

        public DateTime CreateTime { get; set; }

        public int IsDelete { get; set; }

        [Required]
        [StringLength(300)]
        public string Domain { get; set; }
    }

    public enum LoginState
    {
        Invalid=0,
        Valid=1
    }
}
