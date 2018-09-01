using System;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class UsersView
    {
        public int UserID { get; set; }

        public string WorkNo { get; set; }

        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string Email { get; set; }

        public string Ename { get; set; }

        public string TrueName { get; set; }

        public int? Sex { get; set; }

        public string Education { get; set; }

        public string Graduate { get; set; }

        public DateTime? Birthday { get; set; }

        public string qq { get; set; }

        public string Skype { get; set; }

        public string Mobile { get; set; }

        public string HomeTel { get; set; }

        public string HomeAddr { get; set; }

        public string OfficeAddr { get; set; }

        public string OfficeName { get; set; }

        public int Isleave { get; set; }

        public int IsDelete { get; set; }

        public string Remark { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }
        
        public int DeparentId { get; set; }

        public string DeparentName { get; set; }

        public string RoleNames { get; set; }
    }
}
