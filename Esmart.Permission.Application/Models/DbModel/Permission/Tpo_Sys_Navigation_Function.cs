using System;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Navigation_Function
    {
        public int Id { get; set; }

        public int FunctionId { get; set; }

        public int NavigationId { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
