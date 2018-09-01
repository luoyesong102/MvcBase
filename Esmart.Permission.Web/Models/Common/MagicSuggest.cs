using System.Collections.Generic;
using System.Web.Mvc;

namespace Esmart.Permission.Web.Models.Common
{
    public class MagicSuggest
    {
        /// <summary>
        /// [必需]控件ID
        /// </summary>
        public string ControlId { get; set; }
        /// <summary>
        /// [必需]控件在Form中的name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// automatically expands the combo upon focus.
        /// 默认为true
        /// </summary>
        public bool ExpandOnFocus { get; set; }
        public string PlaceHolder { get; set; }
        public IEnumerable<SelectListItem> Data { get; set; }

        public MagicSuggest()
        {
            ExpandOnFocus = true;
        }

        public MagicSuggest(string name, string placeHolder, IEnumerable<SelectListItem> data)
        {
            Name = name;
            PlaceHolder = placeHolder;
            Data = data;
            ExpandOnFocus = true;
        }
    }
}