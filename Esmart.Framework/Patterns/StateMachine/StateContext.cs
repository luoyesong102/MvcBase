using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Patterns.StateMachine
{
    /// <summary>
    /// This object is the context of a state machine.
    /// </summary>
    [Serializable]
    public class StateContext<T>
    {
        [NonSerialized]
        private Queue<T> commandQueue;

        /// <summary>
        /// Instantiates the state context object.
        /// </summary>
        /// <param name="state"></param>
        public StateContext(IState<T> state)
        {
            this.State = state;

            commandQueue = new Queue<T>();
        }

        /// <summary>
        /// Retrieves an item from the command queue.
        /// </summary>
        /// <returns></returns>
        public T GetNext()
        {
            lock (commandQueue)
            {
                if (commandQueue.Count > 0)
                {
                    return commandQueue.Dequeue();
                }
                return default(T);
            }
        }

        /// <summary>
        /// Posts an item to the command queue.
        /// </summary>
        /// <param name="item"></param>
        public void PostEvent(T item)
        {
            lock (commandQueue)
            {
                commandQueue.Enqueue(item);
            }
        }

        /// <summary>
        /// Current state of the state machine.
        /// </summary>
        public IState<T> State
        {
            get;
            set;
        }

        /// <summary>
        /// Each state starts running from here.
        /// </summary>
        public virtual void Request()
        {
            State.Handle(this);
        }
    }
}
