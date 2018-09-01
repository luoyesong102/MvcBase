using System.ComponentModel.DataAnnotations;

namespace Esmart.Permission.Application.Constants
{
    public enum AreaType
    {
        [Display(Name = "早班")]
        Morning = 0,

        [Display(Name = "晚班")]
        Night = 1,

        [Display(Name = "休息")]
        Rest = 2
    }
}
