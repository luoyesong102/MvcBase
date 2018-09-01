using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.ControlModel
{
    public class NavigationFunctionRequest
    {
        public int CreatId { get; set; }
        public int NavigationId { get; set; }
        public List<int> ListFunctionId {  get; set; }
    }
}
