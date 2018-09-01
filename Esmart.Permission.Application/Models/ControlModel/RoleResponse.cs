namespace Esmart.Permission.Application.Models.ControlModel
{
    public class RoleResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsChoice { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Remark { get; set; }
    }
}
