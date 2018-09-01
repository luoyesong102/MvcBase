using AutoMapper;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Permission.Web.Models.Permissions;

namespace Esmart.Permission.Web
{
    public class AutoMapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(x =>
            {
                x.CreateMap<Esmart_Sys_Users, UserEditViewModel>();
                x.CreateMap<UserEditViewModel, Esmart_Sys_Users>();
            });
        }
    }
}