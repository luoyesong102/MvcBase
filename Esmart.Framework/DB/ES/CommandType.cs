using System.ComponentModel.DataAnnotations;

namespace Esmart.Framework.DB
{
    public enum CommandTypes
    {
        [Display(Name = "查询")]
        search = 0,

        [Display(Name = "索引")]
        index = 1,

        [Display(Name = "修改")]
        update = 2,

        [Display(Name = "删除查询")]
        deletebyquery = 3,

        [Display(Name = "删除")]
        delete = 4
        ,
        [Display(Name = "总数")]
        count = 5
        ,
        [Display(Name = "批量")]
        bulk = 6
       
    }
}
