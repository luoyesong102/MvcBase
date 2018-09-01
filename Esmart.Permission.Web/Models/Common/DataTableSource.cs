namespace Esmart.Permission.Web.Models.Common
{
    public class DataTableSource
    {
        public const string TableViewName = "DataTable";
        public const string TableViewNameNew = "IframeDataTable";
        /// <summary>
        /// 查询结果总条数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 获取数据Url
        /// </summary>
        public string Url { get; set; }
     
        /// <summary>
        /// 查询条件Url
        /// </summary>
        public string SearchUrl { get; set; }
        public DataTableSource(string url)
        {
            this.Url = url;
        }
        public DataTableSource(int totalCount, string sourceUrl, string searchUrl)
        {
            this.Count = totalCount;
            this.Url = sourceUrl;
            this.SearchUrl = searchUrl;
        }
    }

    public class DataTableSource<T> : DataTableSource
    {
        public T Filter { get; set; }

        public DataTableSource(string url, T filter)
            : base(url)
        {
            this.Filter = filter;
        }

        public DataTableSource(string searchUrl, string url, T filter)
            : this(url, filter)
        {
            this.SearchUrl = searchUrl;
        }
    }
}