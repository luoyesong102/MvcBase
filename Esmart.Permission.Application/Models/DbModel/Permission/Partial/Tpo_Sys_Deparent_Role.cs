using System.ComponentModel.DataAnnotations.Schema;

namespace Esmart.Permission.Application.Models.DbModel
{
    public partial class Esmart_Sys_Deparent_Role
    {
        [NotMapped]
        public object Tag { get; set; }
    }
}
