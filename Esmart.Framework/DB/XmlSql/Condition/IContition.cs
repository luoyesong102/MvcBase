using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Condition
{
    public interface ICondition
    {
        //ICondition BeginGroup();

        ICondition BeginAndGroup();

        ICondition BeginOrGroup();

        ICondition EndGroup();

        ICondition GroupWithAnd(Func<ICondition, ICondition> condition);

        ICondition GroupWithOr(Func<ICondition, ICondition> condition);

        [Obsolete("已过时，请使用BeginGroup、BeginAndGroup、BeginOrGroup、EndGroup")]
        ICondition Group(bool all= false);

        [Obsolete("已过时，请使用BeginGroup、BeginAndGroup、BeginOrGroup、EndGroup")]
        ICondition GroupWithAnd(bool all = false);

        [Obsolete("已过时，请使用BeginGroup、BeginAndGroup、BeginOrGroup、EndGroup")]
        ICondition GroupWithOr(bool all = false);

        ICondition And(ICondition condition);

        ICondition Or(ICondition condition);

        ConnectFlags ConnectFlag { get; set; }
    }
}
