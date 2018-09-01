using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    public class MessageRequestPage<Tmodel> : DataPage
    {
        /// <summary>
        /// sql查询条件，如where  ID=10
        /// </summary>
        public Tmodel Where { get; set; }
        public string InterFaceId { set; get; }
    }


    public class DataPage
    {
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
    /// 返回的分页信息
    /// </summary>
    /// <typeparam name="Tmodel"></typeparam>
    public class MessageResponsePage<Tmodel>
    {
        /// <summary>
        /// 返回的个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public IEnumerable<Tmodel> Body { get; set; }

        /// <summary>
        /// 错误代码 0或者空正常 1服务内部异常 2服务安全校验失败，3业务异常
        /// </summary>
        public ServerErrcodeEnum Code { set; get; }

        public string Message { set; get; }

    }
}
