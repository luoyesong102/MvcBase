using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Patterns.StateMachine
{
    /// <summary>
    /// This interface is implemented by all states in a state machine.
    /// </summary>
    public interface IState<T>
    {
        void Handle(StateContext<T> context);
    }
}
