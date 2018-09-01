using System;
using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;
using Newtonsoft.Json;

namespace Esmart.Permission.Application.Data
{
    public class RoleDbAction
    {
        #region 单条数据查询
        private const string logFormat="用户：{0}{1}了角色信息,ID：{2}";
        /// <summary>
        /// 获取一条角色信息
        /// 根据角色Id 获取角色信息
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns>Esmart_Sys_Roles</returns>
        public static Esmart_Sys_Roles Find(int roleId)
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Sys_Roles.Find(roleId);
        }

        #endregion

        #region 对象 增加，删除，修改

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="model">角色entity</param>
        /// <returns>true|false</returns>
        public static Esmart_Sys_Roles Add(Esmart_Sys_Roles model)
        {
            var engine = PermissionDb.CreateEngine();

            model.RoleName = model.RoleName.Trim();

            if (engine.Esmart_Sys_Roles.Any(n => n.RoleName == model.RoleName))
                throw new TpoBaseException("角色名称已经存在");

            model.RoleId = (engine.Esmart_Sys_Roles.Max(m => (int?)m.RoleId) ?? 0) + 1;
            var entity = engine.Esmart_Sys_Roles.Add(model);
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = model.CreateId, CreateTime = System.DateTime.Now, EventType = "add", OprTbName = "Esmart_Sys_Roles", OprUserId = model.CreateId, OptDescription = string.Format(logFormat, model.CreateId,"添加",model.RoleId), Remark =JsonConvert.SerializeObject(model) });
            CommonAction.ClearCache();
            return entity;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns>true|false</returns>
        public static bool Delete(int roleId,int optUserId=0)
        {
            var engine = PermissionDb.CreateEngine();
            var entities = engine.Esmart_Sys_Roles.Where(a => a.RoleId == roleId).ToList();
            if (entities.Any(n => n.IsBuiltin))
                throw new TpoBaseException("系统角色不允许删除");
            engine.Esmart_Sys_Roles.RemoveRange(entities);
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = optUserId, CreateTime = System.DateTime.Now, EventType = "delete", OprTbName = "Esmart_Sys_Roles", OprUserId = optUserId, OptDescription = string.Format(logFormat, optUserId, "删除", roleId), Remark = JsonConvert.SerializeObject(entities) });
            CommonAction.ClearCache();
            return true;
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <returns>true|false</returns>
        public static bool Update(RoleModel dto)
        {
            var engine = PermissionDb.CreateEngine();

            var role = engine.Esmart_Sys_Roles.Find(dto.Id);

            if (role.IsBuiltin)
                throw new TpoBaseException("系统角色不允许修改");

            if (engine.Esmart_Sys_Roles.Any(n => n.RoleId != role.RoleId && n.RoleName == dto.Name))
                throw new TpoBaseException("角色名称已经存在");

            role.RoleName = dto.Name.Trim();
            role.Remark = dto.Remark;
            role.StartTime = dto.StartDate ?? DateTime.Now;
            role.EndTime = dto.EndDate ?? role.StartTime.Value.AddYears(99);
            engine.SaveChanges();
            RightLogDb.AddLog(new Esmart_Right_Log { CreateBy = dto.CreatorId, CreateTime = System.DateTime.Now, EventType = "update", OprTbName = "Esmart_Sys_Roles", OprUserId = dto.CreatorId, OptDescription = string.Format(logFormat, dto.CreatorId, "更新", dto.Id), Remark = JsonConvert.SerializeObject(role) });
            CommonAction.ClearCache();
            return true;
        }

        #endregion

        public static List<Esmart_Sys_Roles> GetAllRols()
        {
            var engine = PermissionDb.CreateEngine();
            return engine.Esmart_Sys_Roles.OrderByDescending(n=>n.RoleId).ToList();
        }
    }
}
