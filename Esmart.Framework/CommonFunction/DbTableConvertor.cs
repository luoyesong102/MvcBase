using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Esmart.Framework.Utilities
{
    /// <summary>
    ///     数据表转换类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbTableConvertor<T> where T : new()
    {
        /// <summary>
        ///     将DataTable转换为实体列表
        ///     作者: oldman
        ///     创建时间: 2015年9月13日
        /// </summary>
        /// <param name="dt">待转换的DataTable</param>
        /// <returns></returns>
        public List<T> ConvertToList(DataTable dt)
        {
            // 定义集合  
            var list = new List<T>();

            if (0 == dt.Rows.Count)
            {
                return list;
            }

            // 获得此模型的可写公共属性  
            IEnumerable<PropertyInfo> propertys = new T().GetType().GetProperties().Where(u => u.CanWrite);
            list = ConvertToEntity(dt, propertys);


            return list;
        }

        /// <summary>
        ///     将DataTable的首行转换为实体
        ///     作者: oldman
        ///     创建时间: 2015年9月13日
        /// </summary>
        /// <param name="dt">待转换的DataTable</param>
        /// <returns></returns>
        public T ConvertToEntity(DataTable dt)
        {
            DataTable dtTable = dt.Clone();
            dtTable.Rows.Add(dt.Rows[0].ItemArray);
            return ConvertToList(dtTable)[0];
        }


        private List<T> ConvertToEntity(DataTable dt, IEnumerable<PropertyInfo> propertys)
        {
            var list = new List<T>();
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                var entity = new T();

                //遍历该对象的所有属性  
                foreach (PropertyInfo p in propertys)
                {
                    //将属性名称赋值给临时变量
                    string tmpName = p.Name;

                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (!dt.Columns.Contains(tmpName)) continue;
                    //取值  
                    object value = dr[tmpName];
                    //如果非空，则赋给对象的属性  
                    if (value != DBNull.Value)
                    {
                        p.SetValue(entity, value, null);
                    }
                }
                //对象添加到泛型集合中  
                list.Add(entity);
            }
            return list;
        }

        /// <summary>
                /// 实体类转换成DataTable
                /// </summary>
                /// <param name="modelList">实体类列表</param>
                /// <returns></returns>
        public DataTable FillDataTable(List<T> modelList)
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = CreateData(modelList[0]);
            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }
        /// <summary>
                /// 根据实体类得到表结构
                /// </summary>
                /// <param name="model">实体类</param>
                /// <returns></returns>
        private DataTable CreateData(T model)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
            }
            return dataTable;
        }
    }

}
