using System;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Department_User
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int DeparentId { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
