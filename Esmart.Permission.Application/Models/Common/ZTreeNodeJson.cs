using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.Common
{
    public class ZTreeNodeJson
    {
        public long id { get; set; }

        public string name { get; set; }

        public ICollection<ZTreeNodeJson> children { get; set; }
    }
}