using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.SSOSeanVersion
{
    public interface IUser
    {
        string UserId { get; }

        string UserName { get; }

        string EmailMail { get; }
    }
}
