using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Users
    {
      
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [StringLength(100)]
        public string WorkNo { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(50)]
        public string PassWord { get; set; }

        [StringLength(500)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Ename { get; set; }

        [StringLength(100)]
        public string TrueName { get; set; }

        public int? Sex { get; set; }

        [StringLength(100)]
        public string Education { get; set; }

        [StringLength(100)]
        public string Graduate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? Birthday { get; set; }

        [StringLength(50)]
        public string qq { get; set; }

        [StringLength(100)]
        public string Skype { get; set; }

        [StringLength(50)]
        public string Mobile { get; set; }

        [StringLength(50)]
        public string HomeTel { get; set; }

        [StringLength(200)]
        public string HomeAddr { get; set; }

        [StringLength(200)]
        public string OfficeAddr { get; set; }

        [StringLength(200)]
        public string OfficeName { get; set; }

        public int Isleave { get; set; }

        public int IsDelete { get; set; }

        [StringLength(500)]
        public string Remark { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }

        [NotMapped]
        public string LastToken { get; set; }

        /// <summary>
        /// �ϴε�¼IP
        /// </summary>
        [NotMapped]
        public string LastLoginIP { get; set; }
    }
}
