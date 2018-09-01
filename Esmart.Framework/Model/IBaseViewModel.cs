using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    public interface IBaseViewModel
    {
        bool Success { get; set; }

        string Message { get; set; }
    }
}
