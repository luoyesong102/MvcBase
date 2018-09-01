using System;
using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class MenuResponse
    {
        public int Id { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public int ParentId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Iconurl { get; set; }

        public string InClassName { get; set; }

        public string OutClassName { get; set; }

        public bool IsCheck { get; set; }
        public List<MenuResponse> Children { get; set; }

        public List<FunctionSortInfo> Functions { get; set; }
    }

    public class MenuResponses
    {
        public List<MenuResponse> Menus { get; set; }

        public string HomeUrl { get; set; }

        public int Count
        {
            get { return Menus == null ? 0 : Menus.Count; }
        }
    }

    public class MeunModel
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string Iconurl { get; set; }

        public string InClassName { get; set; }

        public string OutClassName { get; set; }

        public int SortNo { get; set; }

        public int ParentID { get; set; }

        public int CreateId { get; set; }

        public DateTime CreateTime { get; set; }

        public int AppId { get; set; }

        public string Title { get; set; }
    }
}
