using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmart.Framework;
using System.Reflection;

namespace Esmart.Framework.Patterns.Publisher
{
    /// <summary>
    /// Publisher/Subscriber pattern is a variant of Observer pattern. The former uses a subject of 'Enum' type 
    /// as index to subscribe methods of subscribers, while the later uses concrete subject objects to attach
    /// observer objects which derive from an abstract observer class.
    /// 
    /// This infrastructure only works in a single appdomain, and can be used on both server and client sides.
    /// </summary>
    public static class Publisher
    {

        private static Dictionary<Subject, List<Action<object>>> _parameterizedSubscribers = new Dictionary<Subject, List<Action<object>>>();
     
        /// <summary>
        /// Adds an action to the action list of a subject.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="action"></param>
        public static void Attach(Subject subject, Action<object> action)
        {
            if (_parameterizedSubscribers.ContainsKey(subject))
            {
                _parameterizedSubscribers[subject].Add(action);
            }
            else
            {
                var subscriber = new List<Action<object>> { action };
                _parameterizedSubscribers.Add(subject, subscriber);
            }
        }


        /// <summary>
        /// Removes an action from the aciton list of a subject.
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="action"></param>
        public static void Detach(Subject subject, Action<object> action)
        {
            if (_parameterizedSubscribers.ContainsKey(subject))
            {
                _parameterizedSubscribers[subject].Remove(action);
            }
        }

        /// <summary>
        /// Removes the entire action list of a subject.
        /// </summary>
        /// <param name="subject"></param>
        public static void Detach(Subject subject)
        {
            if (_parameterizedSubscribers.ContainsKey(subject))
            {
                _parameterizedSubscribers.Remove(subject);
            }
        }

        /// <summary>
        /// Notifies all the subscribers of an subject.
        /// </summary>
        /// <param name="subject"></param>
        public static void Notify(Subject subject)
        {
            foreach (Action<object> action in _parameterizedSubscribers[subject])
            {
                action(null);
            }
        }

        public static void Notify(Subject subject, object param)
        {
            foreach (Action<object> action in _parameterizedSubscribers[subject])
            {
                action(param);
            }
        }

        //public static void NotifyWithParams(Subject subject, object[] param)
        //{
        //    foreach (Action<object[]> action in _parameterizedSubscribers[subject])
        //    {
        //        action(param);
        //    }
        //}
    }
}
