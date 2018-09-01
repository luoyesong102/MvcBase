using System;

namespace Esmart.Permission.Application.Models.ControlModel
{
   public class DepartmentRequest
    {
        public int DeparentId { get; set; }

        public int SortNo { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public string Remark { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }

        public int IsDelete { get; set; }
    }
}
