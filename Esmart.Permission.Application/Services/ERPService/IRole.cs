using System.Collections.Generic;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application
{
    [ServerAction(ServerType = typeof(RoleService))]
    public interface IRole
    {
        /// <summary>
        /// 获取学习顾问列表
        /// </summary>
        /// <returns>
        /// 如果userid小于1或者为超级管理员，返回所有学习顾问，
        /// 如果用户是学习顾问组长则返回其所属部门的所有学习顾问，
        /// 以上条件不满足，返回用户自身
        /// </returns>
        List<UserLiteDto> GetStudyConsultant(int userId);

        /// <summary>
        /// 获取排课顾问列表
        /// </summary>
        /// <returns>
        /// 如果userid小于1或者为超级管理员，返回所有排课顾问，
        /// 如果用户是学习顾问组长则返回其所属部门的所有排课顾问，
        /// 以上条件不满足，返回用户自身
        /// </returns>
        List<UserLiteDto> GetPkConsultant(int userId);

        /// <summary>
        /// 根据角色名称获取角色相关的所有可用用户
        /// </summary>
        /// <param name="roleName">角色名称</param>
        List<UserLiteDto> GetUsersByRoleName(string roleName);

        /// <summary>
        /// 根据学习顾问返回学习顾问组长
        /// </summary>
        /// <returns>如果根据userid判断出来用户不是学习顾问，那么返回空</returns>
        List<UserLiteDto> GetStudyConsultantLeader(int userId);

        /// <summary>
        /// 根据排课顾问返回排课顾问组长
        /// </summary>
        /// <returns>如果根据userid判断出来用户不是排课顾问，那么返回空</returns>
        List<UserLiteDto> GetScheduleConsultantLeader(int userId);

        /// <summary>
        /// 根据functionkey获取和其相关的用户，如果userId参数大于0，则将获取用户的范围缩小到用户所在的部门级别
        /// </summary>
        List<UserLiteDto> GetUsersByFunctionKey(string functionKey, int userId = 0);

        /// <summary>
        /// 根据functionkey获取和其相关的用户，如果userId参数大于0，则将获取用户的范围缩小到用户所在的部门级别
        /// </summary>
        List<UserLiteDto> GetAllUsersByFunctionKey(string functionKey, int appId, int menuId = 0);

        List<UserLiteDto> GetGroupUsersByFunctionKey(string functionKey, int userId, int menuId, int appId = 0);


    }
}
