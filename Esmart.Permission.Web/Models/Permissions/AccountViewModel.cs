namespace Esmart.Permission.Web.Models.Permissions
{
    public class ModifyUserPassword
    {
        public string UserAccount { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}