using System;

namespace Esmart.Permission.Web.Models.Permissions
{
    public class UserEditViewModel
    {
        public int UserID { get; set; }
        public string WorkNo { get; set; }
        public string Email { get; set; }
        public string Ename { get; set; }
        public string TrueName { get; set; }
        public int? Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string qq { get; set; }
        public string Skype { get; set; }
        public string Mobile { get; set; }
        public string HomeTel { get; set; }
        public string HomeAddr { get; set; }
        public string Remark { get; set; }
        public int? DepartmentId { get; set; }
    }
}