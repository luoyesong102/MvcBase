using System.Collections.Generic;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.DeparentManager
{
    [ServerAction(ServerType = typeof(DepartmentManager))]
    public interface IDepartment
    {
        /// <summary>
        /// 用户ID->RoleId->DepartmentId     by wangligui
        /// </summary>
        List<DepartmentResponse> GetDepartmentsByUserId(int userId, int withUsers);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model">部门信息</param>
        int Add(DepartmentRequest model);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">部门信息</param>
        int Update(DepartmentRequest model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">部门ID</param>
        int Delete(int id);

        /// <summary>
        /// 根据所选部门获取其部门下（包含自身）所有用户
        /// </summary>
        /// <param name="id">部门ID</param>
        /// <returns>返回用户信息集合</returns>
        SoaDataPageResponse<UsersView> GetUsersByDepartList(SoaDataPage<UserSearchModel> id);

        /// <summary>
        /// 根据部门ID获取部门信息
        /// </summary>
        /// <param name="id">部门ID</param>
        DepartmentResponse GetDepartmentResponse(int id);
    }
}
