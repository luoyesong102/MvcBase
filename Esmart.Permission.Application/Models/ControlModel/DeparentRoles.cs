using System.Collections.Generic;

namespace Esmart.Permission.Application.Models.ControlModel
{
   public class DeparentRoles
    {

       public int DeparentId { get; set; }

       public int CreateId { get; set; }

       public List<int> Roles { get; set; }
    }
}
