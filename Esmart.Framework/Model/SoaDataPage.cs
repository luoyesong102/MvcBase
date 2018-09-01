using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Model
{
    public class SoaDataPage<Tmodel> : SoaDataPage
    {
        /// <summary>
        /// sql查询条件，如where  ID=10
        /// </summary>
        public Tmodel Where { get; set; }
        public RequestHeader Header { get; set; }
        public SoaDataPage()
        {
            Header = new RequestHeader();
        }
    }


    public class SoaDataPage
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
    public class SoaDataPageResponse<Tmodel>
    {
        /// <summary>
        /// 返回的个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public IEnumerable<Tmodel> Body { get; set; }

        public ResponseHeader Header { get; set; }

        public SoaDataPageResponse()
        {
            Header = new ResponseHeader();
        }

    }
}
