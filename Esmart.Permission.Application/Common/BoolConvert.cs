using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Esmart.Permission.Application.Common
{
    public class BoolConvert : JsonConverter
    {
        private string[] arrBString { get; set; }

        public BoolConvert()
        {
            arrBString = "是,否".Split(',');
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="BooleanString">将bool值转换成的字符串值</param>
        public BoolConvert(string BooleanString)
        {
            if (string.IsNullOrEmpty(BooleanString))
            {
                throw new ArgumentNullException();
            }
            arrBString = BooleanString.Split(',');
            if (arrBString.Length != 2)
            {
                throw new ArgumentException("BooleanString格式不符合规定");
            }
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool isNullable = IsNullableType(objectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                if (!IsNullableType(objectType))
                {
                    throw new Exception(string.Format("不能转换null value to {0}.", objectType));
                }

                return null;
            }

            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string boolText = reader.Value.ToString();
                    if (boolText.Equals(arrBString[0], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                    else if (boolText.Equals(arrBString[1], StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    //数值
                    return Convert.ToInt32(reader.Value) == 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error converting value {0} to type '{1}'", reader.Value, objectType));
            }
            throw new Exception(string.Format("Unexpected token {0} when parsing enum", reader.TokenType));
        }

        /// <summary>
        /// 判断是否为Bool类型
        /// </summary>
        /// <param name="objectType">类型</param>
        /// <returns>为bool类型则可以进行转换</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }


        public bool IsNullableType(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            return (t.BaseType.FullName == "System.ValueType" && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            bool bValue = (bool)value;

            if (bValue)
            {
                writer.WriteValue(arrBString[0]);
            }
            else
            {
                writer.WriteValue(arrBString[1]);
            }
        }
    }
    public class ChinaDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }
    public class LimitPropsContractResolver : DefaultContractResolver
    {
        string[] props = null;

        bool retain;

        /// <summary>
        /// 构造函数http://www.cnblogs.com/yanweidie/p/4605212.html
        /// </summary>
        /// <param name="props">传入的属性数组</param>
        /// <param name="retain">true:表示props是需要保留的字段  false：表示props是要排除的字段</param>
        public LimitPropsContractResolver(string[] props, bool retain = true)
        {
            //指定要序列化属性的清单
            this.props = props;

            this.retain = retain;
        }

        protected override IList<JsonProperty> CreateProperties(Type type,

        MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list =
            base.CreateProperties(type, memberSerialization);
            //只保留清单有列出的属性
            return list.Where(p =>
            {
                if (retain)
                {
                    return props.Contains(p.PropertyName);
                }
                else
                {
                    return !props.Contains(p.PropertyName);
                }
            }).ToList();
        }
    }
}
