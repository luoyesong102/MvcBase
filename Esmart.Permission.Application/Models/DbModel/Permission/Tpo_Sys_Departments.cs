using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Departments
    {
        [NotMapped]
        public List<Esmart_Sys_Departments> Children { get; set; }
    }
}
