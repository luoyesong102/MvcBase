using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Esmart.Framework.Utilities
{
    /// <summary>
    /// This class provides helper methods that clone an object from an existing object.
    /// </summary>
    public static class CloneMachine
    {
        /// <summary>
        /// Clones an object from an existing object.
        /// </summary>
        /// <param name="itemToClone"></param>
        /// <returns></returns>
        public static object Clone(object itemToClone)
        {
            object retobj; 
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, itemToClone);
                stream.Seek(0, SeekOrigin.Begin);
                retobj = bFormatter.Deserialize(stream);
                stream.Close();
            }
            return retobj;
        }

        /// <summary>
        /// Clones an object of type 'T' from an existing object of type 'T'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CloneGeneric<T>(T source)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, source);

                ms.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
