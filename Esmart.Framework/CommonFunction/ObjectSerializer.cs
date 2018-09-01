using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

namespace Esmart.Framework.Utilities
{
    /// <summary>
    /// class 'ObjectSerializer' provides a set of utilities which serialize objects to xml or binary file, 
    /// or deserialize xml or binary file back to objects.
    /// </summary>
    public sealed class ObjectSerializer
    {
        private ObjectSerializer() { }
        /// <summary>
        /// Serialize the supplied object To memory stream in xml format.
        /// </summary>
        /// <param name="valueToSerializeToMemoryStream"></param>
        /// <returns></returns>
        public static MemoryStream SerializeObjectToMemoryStream(object valueToSerializeToMemoryStream)
        {
            var retVal = new MemoryStream();
            var typeToSerialize = valueToSerializeToMemoryStream.GetType();
            var serializer = new XmlSerializer(typeToSerialize);
            serializer.Serialize(retVal, valueToSerializeToMemoryStream);

            retVal.Seek(0, SeekOrigin.Begin);

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static XmlDocument SerializeObjectToXmlDom(object obj)
        {
            Stream stream = SerializeObjectToMemoryStream(obj);

            XmlDocument xmldom = new XmlDocument();

            xmldom.Load(stream);

            return xmldom;
        }

        private static Type[] RemoveDuplicateType(Type typeOfObject, IEnumerable<Type> types)
        {
            var typeList = new List<Type>();

            foreach (var type in types)
            {
                if (!type.FullName.Equals(typeOfObject.FullName))
                {
                    typeList.Add(type);
                }
            }
            return typeList.ToArray();
        }

        /// <summary>
        /// Serialize derived types to xml stream, SeekOrigin set to Begin.
        /// </summary>
        /// <param name="valueToSerializeToMemoryStream"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static MemoryStream SerializeObjectToMemoryStream(object valueToSerializeToMemoryStream, Type[] types)
        {
            var retVal = new MemoryStream();
            var typeToSerialize = valueToSerializeToMemoryStream.GetType();
            types = RemoveDuplicateType(typeToSerialize, types);
            var serializer = new XmlSerializer(typeToSerialize, types);
            serializer.Serialize(retVal, valueToSerializeToMemoryStream);

            retVal.Seek(0, SeekOrigin.Begin);

            return retVal;
        }

        /// <summary>
        /// Serialize the supplied object To binary stream, SeekOrigin set to Begin.
        /// </summary>
        /// <param name="valueToSerializeToMemoryStream"></param>
        /// <returns></returns>
        public static MemoryStream SerializeObjectToBinaryStream(object valueToSerializeToMemoryStream)
        {
            // Converts object [objectToSerialize] of type [typeOfObjectBeingSerialized] to a memory stream  
            var retVal = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(retVal, valueToSerializeToMemoryStream);
            retVal.Seek(0, SeekOrigin.Begin);
            return retVal;
        }

        /// <summary>
        /// Serializes an object into a file using binary formatter.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="valueToSerialize"></param>
        /// <returns></returns>
        public static bool SerializeObjectToBinaryFile(string fullFileName, object valueToSerialize)
        {
            try
            {
                using (FileStream stream = new FileStream(fullFileName, FileMode.OpenOrCreate))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, valueToSerialize);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (Exception e)
            {
                //Logger.Write(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Serialize the supplied object To Xml stream
        /// </summary>
        /// <param name="valueToSerializeToMemoryStream"></param>
        /// <returns></returns>
        public static string SerializeObjectToXml(object valueToSerializeToMemoryStream)
        {
            using (var memStream = SerializeObjectToMemoryStream(valueToSerializeToMemoryStream))
            using (var reader = new StreamReader(memStream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Serializes an object into a Xml file.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="valueToSerializeToFile"></param>
        /// <returns></returns>
        public static bool SerializeObjectToXmlFile(string fullFileName, object valueToSerializeToFile)
        {
            try
            {
                using(FileStream fs = new FileStream(fullFileName, FileMode.Create))//file existed, use "FileMode.CreateNew" will cause exception
                {
                    using (MemoryStream ms = SerializeObjectToMemoryStream(valueToSerializeToFile))
                    {
                        ms.WriteTo(fs);
                    }
                }
            }
            catch (Exception e)
            {
                //Logger.Write(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Deserialize supplied stream using XmlSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T DeserializeObjectFromStream<T>(Stream stream)
        {
            //todo here always throw exception sometimes 
            var serializer = new XmlSerializer(typeof(T));
            var retVal = (T)serializer.Deserialize(stream);
            return retVal;
        }

        /// <summary>
        /// Convert binary file content to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binFilePath"></param>
        /// <returns></returns>
        public static T DeserializeObjectFromBinaryFile<T>(string binFilePath)
        {
            T obj = default(T);
            using (FileStream fs = new FileStream(binFilePath, FileMode.OpenOrCreate))
            {
                obj = (T)DeserializeObjectFromBinaryStream(fs);
            }
            return obj;
        }

        /// <summary>
        /// Deserialize supplied stream using BinaryFormatter
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object DeserializeObjectFromBinaryStream(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            var retVal = formatter.Deserialize(stream);
            return retVal;
        }

        /// <summary>
        /// Deserialize supplied stream using XmlSerializer to specified type of object
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="typeOfObject"></param>
        /// <returns></returns>
        public static object DeserializeObjectFromStream(Stream stream, Type typeOfObject)
        {
            try
            {
                var serializer = new XmlSerializer(typeOfObject);
                object retVal = serializer.Deserialize(stream);
                return retVal;
            }
            catch(Exception ex)
            {
                //Logger.Write(ex, string.Format("界面发序列化数据出错：\n函数名：{0}\n串行数据内容：{1}",
                //    System.Reflection.MethodInfo.GetCurrentMethod().Name,
                //    stream.ToString()));
                throw;
            }
        }

        /// <summary>
        /// Deserialize supplied stream using XmlSerializer to specified tyep of object 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="typeOfObject"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static object DeserializeObjectFromStream(Stream stream, Type typeOfObject, Type[] types)
        {
            types = RemoveDuplicateType(typeOfObject, types);
            var serializer = new XmlSerializer(typeOfObject, types);
            object retVal = serializer.Deserialize(stream);
            return retVal;
        }


        /// <summary>
        /// Convert supplied string to xml stream, then deserialize the xml stream to specified type of object
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="typeToDeserialize"></param>
        /// <returns></returns>
        public static object DeserializeObjectFromXml(string xml, Type typeToDeserialize)
        {
            using (var memoryStream = new MemoryStream())
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                xmlDoc.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return DeserializeObjectFromStream(memoryStream, typeToDeserialize);
            }
        }


        /// <summary>
        /// Convert xml file content to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlFilePath"></param>
        /// <returns></returns>
        public static T DeserializeObjectFromXmlFile<T>(string xmlFilePath)
        {
            T obj = default(T);
            using (FileStream fs = new FileStream(xmlFilePath, FileMode.OpenOrCreate))
            {
                obj = DeserializeObjectFromStream<T>(fs);
            }
            return obj;
        }
    }
}
