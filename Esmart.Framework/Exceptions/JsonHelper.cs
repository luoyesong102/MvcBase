using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

/// <summary>
/// JSON帮助类。用于将对象转换为Json格式的字符串，或者将Json的字符串转化为对象。
/// </summary>

namespace Esmart.Framework.Exceptions
{
    public static class JsonHelper
    {
        public static string JsonSerializer<T>(T t)
        {

            JsonSerializerSettings jSettings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            jSettings.Converters.Add(new TpoDateTimeConvertor());

            string jsonString = JsonConvert.SerializeObject(t, jSettings);
            return jsonString;

            //System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
            //MemoryStream ms = new MemoryStream();
            //ser.WriteObject(ms, t);
            //string jsonString = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            //ms.Close();
            //return jsonString;

        }


        public static T GetDefaultObject<T>() where T : new()
        {
            var item = new T();
            SerialHelper.SetDefaultValue(item);
            return item;
        }

        /// <summary>
        /// 将对象转化为Json字符串 
        /// </summary>
        /// <typeparam name="T">源类型</typeparam>
        /// <param name="obj">源类型实例</param>
        /// <returns>Json字符串</returns>
        public static string GetJsonFromObj<T>(T obj)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                jsonSerializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// 将Json字符串转化为对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="strJson">Json字符串</param>
        /// <returns>目标类型的一个实例</returns>
        public static T GetObjFromJson<T>(string strJson)
        {
            T obj = Activator.CreateInstance<T>();
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            string strJson_deal = reg.Replace(strJson, matchEvaluator);

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson_deal)))
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(obj.GetType());
                return (T)jsonSerializer.ReadObject(ms);
            } 
        }

        public static object GetObjFromJson(string strJson,Type tp)
        {
            object obj = Activator.CreateInstance(tp);
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            string strJson_deal = reg.Replace(strJson, matchEvaluator);

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strJson_deal)))
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(obj.GetType());
                return  jsonSerializer.ReadObject(ms);
            }
        }


        /// <summary>    
        /// 将时间字符串转为Json时间    
        /// </summary>    
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format(@"/Date({0}+0800)/", ts.TotalMilliseconds);
            return result;
        }
        /// <summary>
        /// 将DataTable转换为JSON字符串
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>JSON字符串</returns>
        public static string GetJsonFromDataTable(DataTable dt)
        {
            StringBuilder JsonString = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"TableInfo\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" +
                                              dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" +
                                              dt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]}");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary> 
        /// 将对象转化为Json字符串 
        /// </summary> 
        /// <param name="obj">源对象</param> 
        /// <returns>json数据</returns> 
        public static string ObjToJson(this object obj)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            return serialize.Serialize(obj);
        }
        public static string ObjToJsonT<T>(this T obj) where T :class
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            return serialize.Serialize(obj);
        }
        /// <summary> 
        /// 将Json字符串转化为对象
        /// </summary> 
        /// <param name="strJson">Json字符串</param> 
        /// <returns>目标对象</returns>
        public static T JsonToObj<T>(this string strJson)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            return serialize.Deserialize<T>(strJson);
        }

        /// <summary> 
        /// 将对象转化为Json字符串(控制深度 )
        /// </summary> 
        /// <param name="obj">源对象</param> 
        /// <param name="recursionDepth">深度</param> 
        /// <returns>json数据</returns> 
        public static string ObjToJson(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            serialize.RecursionLimit = recursionDepth;
            return serialize.Serialize(obj);
        }

        /// <summary> 
        /// 将Json字符串转化为对象(控制深度 )
        /// </summary> 
        /// <param name="strJson">Json字符串</param> 
        /// <param name="recursionDepth">深度</param> 
        /// <returns>目标对象</returns>
        public static T JsonToObj<T>(string strJson, int recursionDepth)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            serialize.RecursionLimit = recursionDepth;
            return serialize.Deserialize<T>(strJson);
        }

        /// <summary> 
        /// 将DataTable转换为JSON字符串
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>json数据</returns> 
        public static string DataTableToJson(DataTable dt)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            int index = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();

                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                dic.Add(index.ToString(), result);
                index++;
            }
            return ObjToJson(dic);
        }
    }

    public class TpoDateTimeConvertor : DateTimeConverterBase
    {
        //public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //{
        //    return DateTime.Parse(reader.Value.ToString());
        //}

        //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //{
        //    writer.WriteValue(((DateTime)value).ToString("dd/MM/yyyy hh:mm"));
        //}

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return DateTime.Parse(reader.Value.ToString());
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}