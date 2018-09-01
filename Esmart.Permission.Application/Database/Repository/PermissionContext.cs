using System.Configuration;
using System.Data.Entity;
using Esmart.Permission.Application.Models;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Permission.Application.Models.SOACommonDB;

namespace Esmart.Permission.Application.Data
{
    public class PermissionContext : DbContext
    {
        internal PermissionContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            
        }

        public PermissionContext() {
            System.Data.Entity.Database.SetInitializer<PermissionContext>(null);
        }
        public virtual DbSet<Esmart_Sys_Deparent_Role> Esmart_Sys_Deparent_Role { get; set; }
        public virtual DbSet<Esmart_Sys_Department_User> Esmart_Sys_Department_User { get; set; }
        public virtual DbSet<Esmart_Sys_Departments> Esmart_Sys_Departments { get; set; }
        public virtual DbSet<Esmart_Sys_Functions> Esmart_Sys_Functions { get; set; }
        public virtual DbSet<Esmart_Sys_Navigation_Function> Esmart_Sys_Navigation_Function { get; set; }
        public virtual DbSet<Esmart_Sys_Navigations> Esmart_Sys_Navigations { get; set; }
        public virtual DbSet<Esmart_Sys_Roles> Esmart_Sys_Roles { get; set; }
        public virtual DbSet<Esmart_Sys_Users> Esmart_Sys_Users { get; set; }
        public virtual DbSet<Esmart_Sys_LogInfo> Esmart_Sys_LogInfo { get; set; }
        public virtual DbSet<Esmart_Right_Log> Esmart_Right_Log { get; set; }
        public virtual DbSet<Esmart_Sys_Role_Navigation_Function> Esmart_Sys_Role_Navigation_Function { get; set; }
        public virtual DbSet<Esmart_Sys_Role_Navigations> Esmart_Sys_Role_Navigations { get; set; }
        public virtual DbSet<Esmart_Sys_User_Roles> Esmart_Sys_User_Roles { get; set; }

        public virtual DbSet<Esmart_Sys_User_Navigations> Esmart_Sys_User_Navigations { get; set; }
        public virtual DbSet<Esmart_Sys_User_Navigation_Function> Esmart_Sys_User_Navigation_Function { get; set; }
    }

    public class SoaCommonDBContext : DbContext
    {
        internal SoaCommonDBContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            System.Data.Entity.Database.SetInitializer<SoaCommonDBContext>(null);
        }

        public virtual DbSet<AppInfo> AppInfo { get; set; }

        public virtual DbSet<AppRequest> AppRequest { get; set; }
        
        public virtual DbSet<RequestInfo> RequestInfo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppInfo>()
                .Property(e => e.CreateID)
                .IsUnicode(false);

            modelBuilder.Entity<AppRequest>()
                .Property(e => e.CreateID)
                .IsUnicode(false);

            modelBuilder.Entity<RequestInfo>()
                .Property(e => e.RequestType)
                .IsUnicode(false);

            modelBuilder.Entity<RequestInfo>()
                .Property(e => e.RequestUrl)
                .IsUnicode(false);

            modelBuilder.Entity<RequestInfo>()
                .Property(e => e.UserID)
                .IsUnicode(false);

            modelBuilder.Entity<RequestInfo>()
                .Property(e => e.GetCacheKey)
                .IsUnicode(false);

            modelBuilder.Entity<RequestInfo>()
                .Property(e => e.RemoveCacheKey)
                .IsUnicode(false);
        }
    }

    public class PermissionDb
    {
        public static PermissionContext CreateEngine()
        {
            return new PermissionContext(ConfigurationManager.ConnectionStrings[DbConst.Permission].ConnectionString);
        }
    }

    public class SoaCommonDB
    {
        public static SoaCommonDBContext CreateEngine()
        {
            return new SoaCommonDBContext(ConfigurationManager.ConnectionStrings[DbConst.SoaCommonDB].ConnectionString);
        }
    }

}
