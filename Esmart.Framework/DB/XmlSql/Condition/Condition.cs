using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Esmart.Framework.Condition
{
    [Serializable]
    public class Condition : ICondition, ISerializable
    {

        private List<ICondition> _criteriaPool;

        public Condition()
        {
            _criteriaPool = new List<ICondition>();
        }
        public Condition(IField left, IField rigth, string opt)
            : this()
        {
            this.Left = left;
            this.Rigth = rigth;
            this.Operator = opt;
        }

        public IField Left
        {
            get;
            set;
        }
        public IField Rigth
        {
            get;
            set;
        }

        public string Operator
        {
            get;
            set;
        }

        internal bool GroupAll
        {
            get;
            set;
        }

        public bool IsUnitary
        {
            get;
            set;
        }

        internal GroupConnectFlags GroupConnectFlag
        {
            get;
            set;
        }

        public ICondition BeginAndGroup()
        {
            this._criteriaPool.Add(new Condition
            {
                ConnectFlag = ConnectFlags.AndGroup
            });
            return this;
        }

        public ICondition BeginOrGroup()
        {
            this._criteriaPool.Add(new Condition
            {
                ConnectFlag = ConnectFlags.OrGroup
            });
            return this;
        }

        public ICondition EndGroup()
        {
            this._criteriaPool.Add(new Condition
            {
                ConnectFlag = ConnectFlags.EndGroup
            });
            return this;
        }

        public ICondition GroupWithAnd(Func<ICondition, ICondition> condition)
        {
            this._criteriaPool.Add(new Condition
            {
                ConnectFlag = ConnectFlags.AndGroup
            });
            var that = condition(this);
            return that.EndGroup();
        }

        public ICondition GroupWithOr(Func<ICondition, ICondition> condition)
        {
            this._criteriaPool.Add(new Condition
            {
                ConnectFlag = ConnectFlags.OrGroup
            });
            var that = condition(this);
            return that.EndGroup();
        }

        /// <summary>
        /// 对前面的条件进行GROUP
        /// </summary>
        /// <param name="all">true:将前面所有的条件进行GROUP；false:将前面最近的一组条件进行GROUP</param>
        /// <returns></returns>
        public ICondition Group(bool all = false)
        {
            this._criteriaPool.Add(new Condition()
            {
                ConnectFlag = ConnectFlags.Group,
                GroupAll = all,
                GroupConnectFlag = GroupConnectFlags.None
            });
            return this;
        }

        /// <summary>
        /// 对前面的条件进行GROUP，并与之前的GROUP以AND连接
        /// </summary>
        /// <param name="all">true:将前面所有的条件进行GROUP；false:将前面最近的一组条件进行GROUP</param>
        public ICondition GroupWithAnd(bool all = false)
        {
            this._criteriaPool.Add(new Condition()
            {
                ConnectFlag = ConnectFlags.Group,
                GroupAll = all,
                GroupConnectFlag = GroupConnectFlags.And
            });
            return this;
        }

        /// <summary>
        /// 对前面的条件进行GROUP，并与之前的GROUP以OR连接
        /// </summary>
        /// <param name="all">true:将前面所有的条件进行GROUP；false:将前面最近的一组条件进行GROUP</param>
        public ICondition GroupWithOr(bool all = false)
        {
            this._criteriaPool.Add(new Condition()
            {
                ConnectFlag = ConnectFlags.Group,
                GroupAll = all,
                GroupConnectFlag = GroupConnectFlags.Or
            });
            return this;
        }

        /// <summary>
        /// 将两个条件以AND进行连接
        /// </summary>
        public ICondition And(ICondition condition)
        {
            condition.ConnectFlag = ConnectFlags.And;
            this._criteriaPool.Add(condition);
            return this;
        }

        /// <summary>
        /// 将两个条件以OR进行连接
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ICondition Or(ICondition condition)
        {
            condition.ConnectFlag = ConnectFlags.Or;
            this._criteriaPool.Add(condition);
            return this;
        }

        public ConnectFlags ConnectFlag
        {
            get;
            set;
        }

        private readonly static string GROUP_FLAG = "::GROUP::";
        public override string ToString()
        {
            string result = "";
            foreach (Condition condition in _criteriaPool)
            {
                if (condition.ConnectFlag == ConnectFlags.Group)
                {
                    switch (condition.GroupConnectFlag)
                    {
                        case GroupConnectFlags.And:
                            result = InsertedGroup(result, condition.GroupAll ? "(" : " And (", condition.GroupAll ? 0 : LastGroupIndex(result), condition.GroupAll) + ")" + GROUP_FLAG;
                            break;
                        case GroupConnectFlags.Or:
                            result = InsertedGroup(result, condition.GroupAll ? "(" : " Or (", condition.GroupAll ? 0 : LastGroupIndex(result), condition.GroupAll) + ")" + GROUP_FLAG;
                            break;
                        case GroupConnectFlags.None:
                            result = " ( " + result + " ) " + GROUP_FLAG; ;
                            break;
                        default:
                            break;
                    }
                }
                else if (condition.ConnectFlag == ConnectFlags.And || condition.ConnectFlag == ConnectFlags.Or || condition.ConnectFlag == ConnectFlags.AndGroup || condition.ConnectFlag == ConnectFlags.OrGroup)
                {
                    if (condition.Left == null && condition.Operator == null && condition.Rigth == null)
                    {
                        result += condition.ConnectFlag.ToSqlString() + condition;
                    }
                    else
                    {
                        var flag = (result == "" || result.Trim().Last() == '(') ? "" : condition.ConnectFlag.ToSqlString();
                        result += flag +
                                  condition.Left.Seed +
                                  condition.Operator +
                                  (condition.IsUnitary ? "" : condition.Rigth.Seed);
                    }
                }
                else if (condition.ConnectFlag == ConnectFlags.EndGroup)
                {
                    if (condition.Left == null && condition.Operator == null && condition.Rigth == null)
                    {
                        result += condition.ConnectFlag.ToSqlString() + condition;
                    }
                    else
                    {
                        result = result.Trim();
                        if (result.EndsWith("("))
                        {
                            result = result.Remove(result.Length - 1);
                        }
                        if(result.EndsWith("AND", StringComparison.OrdinalIgnoreCase))
                        {
                            result = result.Remove(result.Length - 3);
                        }
                        if (result.EndsWith("OR", StringComparison.OrdinalIgnoreCase))
                        {
                            result = result.Remove(result.Length - 2);
                        }
                        result += condition.ConnectFlag.ToSqlString() +
                                    condition.Left.Seed +
                                    condition.Operator +
                                    (condition.IsUnitary ? "" : condition.Rigth.Seed);
                    }
                }
            }
            result = result.Trim();
            if (result.StartsWith("and", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(4);
            }
            else if(result.StartsWith("or", StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(3);
            }
            var whereString = result.Replace(GROUP_FLAG, string.Empty).Replace("(  )  And ", "").Replace("(  )  Or ", "").Replace("( AND", "(").Replace("( OR", "(");
            if (whereString.Count(c => c.Equals('(')) != whereString.Count(c => c.Equals(')')))
                throw new Exception("Condition的括号不匹配，请确定对应的BeginxGroup是否有对应的EndGroup，及确定没有多余的EndGroup");
            return whereString;
        }

        private string InsertedGroup(string refStr, string groupStr, int index, bool groupAll)
        {
            var result = refStr.Insert(index, groupStr);
            if (!groupAll)
            {
                string andKey = " And ";
                string orKey = " Or ";
                string startStr = result.Substring(0, index + groupStr.Length);
                string endStr = result.Substring(index + groupStr.Length);
                if (endStr.IndexOf(andKey, StringComparison.Ordinal) == 0)
                {
                    return startStr + result.Substring(index + groupStr.Length + andKey.Length);
                }
                if (endStr.IndexOf(" Or ", StringComparison.Ordinal) == 0)
                {
                    return startStr + result.Substring(index + groupStr.Length + orKey.Length);
                }
                //string startStr = result.Substring(0, index + groupStr.Length);
                //result = Regex.Replace(result, string.Format("({0})(\\s+?And|Or\\s+?)([\\s\\S]+)", startStr), "$1$3");
            }
            //var andKey = " And ";
            //var orKey = " Or ";
            //var fAndIndex = result.IndexOf(andKey, index + groupStr.Length);
            //var fOrIndex = result.IndexOf(orKey, index + groupStr.Length);
            //if (fAndIndex > 0)
            //{
            //    result = result.Substring(0, fAndIndex) + result.Substring(fAndIndex + andKey.Length, result.Length - fAndIndex - andKey.Length);
            //}
            //if (fOrIndex>0)
            //{
            //    result = result.Substring(0, fOrIndex) + result.Substring(fOrIndex + orKey.Length, result.Length - fOrIndex - orKey.Length);
            //}
            return result;
        }

        private int LastGroupIndex(string cStr)
        {
            var sIndex = cStr.LastIndexOf(GROUP_FLAG);
            return sIndex >= 0 ? sIndex + GROUP_FLAG.Length : -1;
        }


        internal enum GroupConnectFlags
        {
            And,
            Or,
            None
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
