using System;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class FunctionModel
    {
        public int AppId { get; set; }

        public int FunctionId { get; set; }

        public string FunctionName { get; set; }

        public string Remark { get; set; }

        public string FunctionKey { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }
    }


    public class MenuFunctionModel
    {
        public int NavigationId { get; set; }

        public int? FunctionId { get; set; }

        public string FunctionKey { get; set; }


        public string FunctionName { get; set; }
    }
}
