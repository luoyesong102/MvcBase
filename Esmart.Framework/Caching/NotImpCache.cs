using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Caching
{
    public class NotImpCache:ICache
    {
        public bool Add<T>(string key, T value)
        {
            return false;
        }

        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            return false;
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            return false;
        }

        public void FlushAll()
        {
            
        }

        public T Get<T>(string key)
        {
            return default(T);
        }

        public IDictionary<string, Tvalue> GetAll<Tvalue>(IEnumerable<string> keys)
        {
            return null;
        }

        public bool Remove(string key)
        {
            return false;
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
           
        }

        public bool Replace<T>(string key, T value)
        {
            return false;
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            return false;
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return false;
        }

        public bool Set<T>(string key, T value)
        {
            return false;
        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return false;
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return false;
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
           
        }

        public void AddItemToList(string key, string value)
        {
           
        }

        public List<string> GetAllItemsFromList(string key)
        {
            return null;
        }

        public int RemoveItemFromList(string key, string value)
        {
            return -1;
        }


        public void RemoveItemFromList(string key, List<string> values)
        {
            
        }


        public string ConnectString
        {
            get { return string.Empty; }
        }
    }
}
