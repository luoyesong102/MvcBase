/*
 ===============================================================================
 * 作者        ：chalei.wu
 * 编写时间    :2014-11-23
 * 修改历史记录：
 * 存在的bug   ：
 * 待优化方案  ：
 ===============================================================================
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Caching
{
    /// <summary>
    /// 对缓存的基础操作
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 根据key  保存value
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="key">唯一key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        bool Add<T>(string key, T value);

        /// <summary>
        /// 按照时间有效期放入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        bool Add<T>(string key, T value, DateTime expiresAt);

        /// <summary>
        /// 按照TimeSpan放入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        bool Add<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// `清空所有缓存
        /// </summary>
        void FlushAll();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 多个key获取数据
        /// </summary>
        IDictionary<string, Tvalue> GetAll<Tvalue>(IEnumerable<string> keys);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
      
        bool Remove(string key);

        /// <summary>
        /// 根据多个key删除缓存
        /// </summary>
        /// <param name="keys"></param>
        void RemoveAll(IEnumerable<string> keys);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        bool Replace<T>(string key, T value);

        /// <summary>
        /// 设置过期时间跟新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        bool Replace<T>(string key, T value, DateTime expiresAt);

        /// <summary>
        /// 设置过期时间跟新缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        bool Replace<T>(string key, T value, TimeSpan expiresIn);

        /// <summary>
        /// 保存缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Set<T>(string key, T value);

        /// <summary>
        /// 根据过期时间保存缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        bool Set<T>(string key, T value, DateTime expiresAt);


        /// <summary>
        /// 根据过期时间保存缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Set<T>(string key, T value, TimeSpan expiresIn);


        /// <summary>
        /// 多批量设置缓存
        /// </summary>
        /// <returns></returns>
        void SetAll<T>(IDictionary<string, T> values);

        /// <summary>
        /// 根据key 把value放入list列表中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddItemToList(string key, string value);

        /// <summary>
        /// 根据key获取list
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        List<string> GetAllItemsFromList(string key);

        /// <summary>
        /// 删除列表中的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int RemoveItemFromList(string key, string value);

        /// <summary>
        /// 根据values删除 key里面的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        void RemoveItemFromList(string key, List<string> values);

        /// <summary>
        /// 连接字符串
        /// </summary>
        string ConnectString { get; }
    }
}
