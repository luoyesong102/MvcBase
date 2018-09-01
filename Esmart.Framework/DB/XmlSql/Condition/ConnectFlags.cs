using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Condition
{
    [Flags]
    public enum ConnectFlags
    {
        And =0x1,
        Or =0x2,
        Group =0x4,
        EndGroup = 0x8,
        AndGroup = 0x16,
        OrGroup = 0x32
    }

    public static class ConnectFlagsExtend
    {
        public static string ToSqlString(this ConnectFlags flag)
        {
            switch (flag)
            {
                case ConnectFlags.And:
                    return " AND ";
                case ConnectFlags.Or:
                    return " OR ";
                case ConnectFlags.Group:
                    return " (";
                case ConnectFlags.EndGroup:
                    return ") ";
                case ConnectFlags.AndGroup:
                    return " AND (";
                case ConnectFlags.OrGroup:
                    return " OR (";
                default:
                    return string.Empty;
            }
        }
    }
}
