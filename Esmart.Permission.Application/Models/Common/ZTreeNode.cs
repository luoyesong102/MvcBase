using System.Collections.Generic;
using Newtonsoft.Json;

namespace Esmart.Permission.Application.Models.Common
{
    public class ZTreeNode
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("pId")]
        public int ParentId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("open")]
        public bool? Open { get; set; }

        [JsonProperty("checked")]
        public bool? Checked { get; set; }

        [JsonProperty("chkDisabled")]
        public bool? Disabled { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("iconSkin")]
        public string IconSkin { get; set; }

        [JsonProperty("children")]
        public List<ZTreeNode> Children { get; set; }
    }
}