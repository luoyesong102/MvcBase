namespace Esmart.Permission.Application.Models.ControlModel
{
    public class AppShortInfo
    {
        public int AppId { get; set; }

        public string AppName { get; set; }

        /// <summary>
        /// 公共域名
        /// </summary>
        public string Domain { get; set; }
    }
}
