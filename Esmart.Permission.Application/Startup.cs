using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Constants;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;

namespace Esmart.Permission.Application
{
    public static class Startup
    {
        public static void Configuration()
        {
            
            ConfigAutoMapper();
            //CreateBuiltinRole();
           
        }

        static void ConfigAutoMapper()
        {
            Mapper.CreateMap<UpdateUserDto, Esmart_Sys_Users>();
            Mapper.CreateMap<Esmart_Sys_Users, Esmart_Sys_Users>()
                .ForMember(n => n.CreateId, opt => opt.Ignore())
                .ForMember(n => n.CreateTime, opt => opt.Ignore())
                .ForMember(n => n.IsDelete, opt => opt.Ignore())
                .ForMember(n => n.Isleave, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition(n => !n.IsSourceValueNull));
        }

        private static void CreateBuiltinRole()
        {
            var builtinRoles = new HashSet<string>(BuiltinRoles.All);

            var dbContext = PermissionDb.CreateEngine();
            var dbTable = dbContext.Esmart_Sys_Roles;
            var maxRoleId = dbTable.Max(n => n.RoleId);

            foreach (var dbRole in dbTable.ToArray())
            {
                if (builtinRoles.Contains(dbRole.RoleName))
                {
                    dbRole.IsBuiltin = true;
                    dbRole.EndTime = new DateTime(9999, 1, 1);
                    if (dbRole.StartTime > DateTime.Today)
                        dbRole.StartTime = DateTime.Now.AddYears(-1);
                    builtinRoles.Remove(dbRole.RoleName);
                }
                else
                {
                    dbRole.IsBuiltin = false;
                }
            }

            foreach (var bRole in builtinRoles)
            {
                dbTable.Add(new Esmart_Sys_Roles
                {
                    RoleId = ++maxRoleId,
                    RoleName = bRole,
                    StartTime = DateTime.Now.AddYears(-1),
                    EndTime = new DateTime(9999, 1, 1),
                    CreateId = 1,
                    CreateTime = DateTime.Now,
                    IsBuiltin = true
                });
            }

            dbContext.SaveChanges();
        }
        
    }
}
