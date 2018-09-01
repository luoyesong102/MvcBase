using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{

    /// <summary>
    /// 分页查询
    /// </summary>
    public class DBPage
    {
        /// <summary>
        /// sql查询条件，如where  ID=10
        /// </summary>
        public object Where { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 显示多少数据
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 按照什么排序
        /// </summary>
        public Sort SortCol { get; set; }
    }

    /// <summary>
    /// 排序
    /// </summary>
    public enum Sort
    {
        Asc = 0,
        Desc = 1
    }
}
