using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Esmart.Framework.Patterns.Ballot
{
    public delegate TResult FuncOut<T1, T2, T3, T4, TResult>(
	    T1 arg1,
	    T2 arg2,
	    T3 arg3,
	    out T4 arg4
    );
    /// <summary>
    /// In chamber task, in many cases, before taking some action, 
    /// it needs to check several conditions. Only when one of them, 
    /// or all of them are satisfied, will the action be conducted. 
    /// Usually, those conditions are hardware related. In no doubt, we can check these conditions one by one. 
    /// But if in the future, a new hard is introduced, and the new hardware is one of the conditions. 
    /// Then the original code must be changed accordingly. 
    /// If the new condition is checked in many places, all the code conerning the new hardware must be changed. T
    /// his makes code maintenance very hard and very inefficient.
    /// A better solution is to use a variant of Observer design pattern - Ballot. 
    /// In Ballot mechanism, when checking a series conditions, it doesn't directly check those conditions one by one. 
    /// Instead, It publishes a subject to which all conditions are subscribed. 
    /// This way, all subscribed conditions will be called back and return a "True" or "False". 
    /// When a new piece of hardward is introduced and it may be needed in many condition checks, 
    /// we simply subscribe this new hardward to different subjects. Thus the original code will not be affected at all, 
    /// and even doens't know if any new hard has been introduced. This will significantly enhance the code flexibility, reusability and maintenance.
    /// </summary>
    public class Ballot
    {

        /// <summary>
        /// ballot dictionary
        /// </summary>
        static Dictionary<BallotType, List<FuncOut<object, object, object, object, bool?>>> _checkFuncDic
            = new Dictionary<BallotType, List<FuncOut<object, object, object, object, bool?>>>();

        /// <summary>
        /// poll ticket subscribe
        /// </summary>
        /// <param name="ballotType"></param>
        /// <param name="conditionDelegate"></param>
        public static void Subscribe(BallotType ballotType, FuncOut<object, object, object, object, bool?> conditionDelegate)
        {
            if (!_checkFuncDic.Keys.Contains<BallotType>(ballotType))
                _checkFuncDic.Add(ballotType, new List<FuncOut<object, object, object, object, bool?>>());
            if (_checkFuncDic[ballotType].Contains<FuncOut<object, object, object, object, bool?>>(conditionDelegate))
                throw new Exception(string.Format("Func:{0} for BallotType:{1} reduplicative!", conditionDelegate.Method.Name, ballotType.ToString()));
            _checkFuncDic[ballotType].Add(conditionDelegate);
        }

        /// <summary>
        /// checking a series conditions.
        /// When voters vote for a ballot type, they do not change anything; they only check some conditions. It means
        /// race condition will not arise. Thus, ballot mechanism can be used by multiple threads at the same time, 
        /// without mutual exclusion being needed, that is, no lock is needed.
        /// </summary>
        /// <param name="ballotType"></param>
        /// <param name="ballotRelation"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public static bool? Vote(BallotType ballotType, BallotRelation ballotRelation, object arg1, object arg2, object arg3, out object arg4)
        {
            arg4 = string.Empty;

            if (!_checkFuncDic.Keys.Contains<BallotType>(ballotType))
                return null;
            bool isCheckPass = default(bool);
            switch (ballotRelation)
            {
                case BallotRelation.AND:
                    foreach (var item in _checkFuncDic[ballotType])
                    {
                        bool? result = item(arg1, arg2, arg3, out arg4);
                        if (!result.HasValue)
                        {
                            continue;
                        }
                        if (!result.Value)
                        {
                            return false;
                        }
                    }
                    isCheckPass = true;
                    break;

                case BallotRelation.NONE:
                    foreach (var item in _checkFuncDic[ballotType])
                    {
                        bool? result = item(arg1, arg2, arg3, out arg4);
                        if (!result.HasValue)
                        {
                            continue;
                        }
                        if (result.Value)
                        {
                            return false;
                        }
                    }
                    isCheckPass = true;
                    break;

                case BallotRelation.OR:
                    foreach (var item in _checkFuncDic[ballotType])
                    {
                        bool? result = item(arg1, arg2, arg3, out arg4);
                        if (!result.HasValue)
                        {
                            continue;
                        }
                        if (result.Value)
                        {
                            return true;
                        }
                    }
                    isCheckPass = false;
                    break;
            }

            return isCheckPass;
        }


        /// <summary>
        /// checking a series conditions
        /// </summary>
        /// <param name="ballotType"></param>
        /// <param name="ballotRelation"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public static bool? Vote(BallotType ballotType, BallotRelation ballotRelation, object arg1, object arg2, object arg3)
        {
            object result = string.Empty;

            return Vote(ballotType,  ballotRelation,arg1, arg2, arg3, out result );
        }

        /// <summary>
        /// checking a series conditions
        /// </summary>
        /// <param name="ballotType"></param>
        /// <param name="ballotRelation"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static bool? Vote(BallotType ballotType, BallotRelation ballotRelation, object arg1, object arg2)
        {
            object result = string.Empty;

            return Vote(ballotType, ballotRelation, arg1, arg2, null, out result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ballotType"></param>
        /// <param name="ballotRelation"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static bool? Vote(BallotType ballotType, BallotRelation ballotRelation, object arg1)
        {
            object result = string.Empty;

            return Vote(ballotType, ballotRelation, arg1, null, null, out result);
        }

        /// <summary>
        /// checking a series conditions
        /// </summary>
        /// <param name="ballotType"></param>
        /// <param name="ballotRelation"></param>
        /// <returns></returns>
        public static bool? Vote(BallotType ballotType, BallotRelation ballotRelation)
        {
            object result = string.Empty;
            
            return Vote(ballotType, ballotRelation, null, null, null, out result);
        }
    }
}
