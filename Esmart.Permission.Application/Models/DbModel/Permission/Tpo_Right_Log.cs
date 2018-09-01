
namespace Esmart.Permission.Application.Models.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class Esmart_Right_Log
    {
        [Key]
        public int Id { get; set; }
        public string OprTbName { get; set; }
        public Nullable<int> OprUserId { get; set; }
        public string EventType { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public string OptDescription { get; set; }
    }
}
