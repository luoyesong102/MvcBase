using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Esmart.Framework.DB.Dapper
{
    [Serializable]
    public class Parameter
    {
        private string parameterName;
        private object parameterValue;
        private DbType? parameterDbType;
        private int? parameterSize;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <param name="parameterDbType"></param>
        /// <param name="parameterSize"></param>
        public Parameter(string parameterName, object parameterValue)
        {
            this.parameterName = parameterName;
            this.parameterValue = parameterValue;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }


        /// <summary>
        /// 参数值
        /// </summary>
        public object ParameterValue
        {
            get { return parameterValue; }
            set { parameterValue = value; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public DbType? ParameterDbType
        {
            get { return parameterDbType; }
            set { parameterDbType = value; }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public int? ParameterSize
        {
            get { return parameterSize; }
            set { parameterSize = value; }
        }
    }
}
