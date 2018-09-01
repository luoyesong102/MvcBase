using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Patterns.StateMachine
{
    public abstract class ACommand
    {
        public ACommand(params object[] parameters)
        {
            Parameters = parameters;
        }

        public object[] Parameters
        {
            get;
            private set;
        }

        public abstract void Execute(object context);
    }
}
