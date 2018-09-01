namespace Esmart.Permission.Application.Models.ControlModel
{
    /// <summary>
    /// 功能的简单信息
    /// </summary>
    public class FunctionSortInfo
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public bool IsCheck { get; set; }
        public string Name { get; set; }
    }
}
