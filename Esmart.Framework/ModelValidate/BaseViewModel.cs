using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.ModelValidate
{
    public class BaseViewModel : IBaseViewModel
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public void SetSuccess(string message = "操作成功")
        {
            this.Success = true;
            this.Message = message;
        }

        public void SetFailed(string message = "操作失败")
        {
            this.Success = false;
            this.Message = message;
        }
    }
}
