using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.DeparentManager
{
    public class DepartmentManager : IDepartment
    {
        /// <summary>
        /// 根据用户Id获取部门列表
        /// 用户Id->RoleId->DepartmentId
        /// </summary>
        public List<DepartmentResponse> GetDepartmentsByUserId(int userId, int withUsers)
        {
            var departments = DepartmentDbAction.GetDepartments(CommonAction.IsSysAdmin(userId) ? null : (int?)userId);
            var result = new List<DepartmentResponse>(10);
            DepartmentDto(departments, result, withUsers > 0);
            return result;
        }

        //dto转换
        private static void DepartmentDto(IEnumerable<Esmart_Sys_Departments> departments, List<DepartmentResponse> result, bool withUser)
        {
            foreach (var item in departments)
            {
                var dto = new DepartmentResponse
                {
                    DeparentId = item.DeparentId,
                    Name = item.Name,
                    ParentId = item.ParentId
                };

                result.Add(dto);

                if (item.Children == null || item.Children.Count == 0)
                {
                    dto.Children = new List<DepartmentResponse>();
                }
                else
                {
                    dto.Children = new List<DepartmentResponse>(item.Children.Count);
                    DepartmentDto(item.Children, dto.Children, withUser);
                }

                if (withUser)
                {
                    var users = DepartmentUserDbAction.GetUsersInDepartment(item.DeparentId, false);
                    dto.Users = users.Select(n => new DepartmentUserResponse
                    {
                        UserID = n.UserID,
                        UserName = n.TrueName
                    }).ToList();
                }
            }
        }

        #region 基础操作：添加，修改，删除

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns>返回值</returns>
        public int Add(DepartmentRequest request)
        {
            var model = new Esmart_Sys_Departments();
            if (request != null)
            {
                model.DeparentId = request.DeparentId;
                model.CreateId = request.CreateId;
                model.CreateTime = request.CreateTime;
                model.Name = request.Name;
                model.ParentId = request.ParentId;
                model.Remark = request.Remark;
                model.SortNo = request.SortNo;
                model.IsDelete = request.IsDelete;
                DepartmentDbAction.Add(model);
                return model.DeparentId;
            }
            return -1;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns>返回值</returns>
        public int Update(DepartmentRequest request)
        {
            var result = 0;
            var model = new Esmart_Sys_Departments();
            if (request != null)
            {
                model.DeparentId = request.DeparentId;
                model.CreateId = request.CreateId;
                model.CreateTime = request.CreateTime;
                model.Name = request.Name;
                model.ParentId = request.ParentId;
                model.Remark = request.Remark;
                model.SortNo = request.SortNo;
                model.IsDelete = request.IsDelete;
                result = DepartmentDbAction.Update(model);
            }
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">部门ID</param>
        /// <returns>返回值</returns>
        public int Delete(int id)
        {
            return DepartmentDbAction.Delete(id);
        }

        #endregion

        public DepartmentResponse GetDepartmentResponse(int id)
        {
            var _response = new DepartmentResponse();
            var _sysDepartments = DepartmentDbAction.GetDeparentById(id);
            _response.DeparentId = _sysDepartments.DeparentId;
            _response.Name = _sysDepartments.Name;
            _response.Remark = _sysDepartments.Remark;
            _response.CreateId = _sysDepartments.CreateId;
            _response.CreateTime = _sysDepartments.CreateTime;
            _response.IsDelete = _sysDepartments.IsDelete;
            _response.ParentId = _sysDepartments.ParentId;
            return _response;
        }

        public SoaDataPageResponse<UsersView> GetUsersByDepartList(SoaDataPage<UserSearchModel> filter)
        {
            SoaDataPageResponse<UsersView> response;

            if (filter.Where.DeapartmentId != 0)
            {
                response = DepartmentDbAction.GetUsersByDepartList(filter);
            }
            else
            {
                IQueryable<UsersView> query;

                if (CommonAction.IsSysAdmin(filter.Where.UserId))
                {
                    query = DepartmentUserDbAction.GetAllUsers(true).AsQueryable();
                }
                else
                {
                    var userList = new List<UsersView>(100);

                    var departments = DepartmentDbAction.GetDepartments(filter.Where.UserId);

                    foreach (var department in departments)
                    {
                        var users = DepartmentUserDbAction.GetUsersInDepartment(department.DeparentId, true);
                        userList.AddRange(users);
                    }

                    query = userList.AsQueryable();
                }

                if (!string.IsNullOrWhiteSpace(filter.Where.TrueName))
                    query = query.Where(n => n.TrueName != null && n.TrueName.Contains(filter.Where.TrueName));

                if (!string.IsNullOrWhiteSpace(filter.Where.Ename))
                    query = query.Where(n => n.Ename != null && n.Ename.Contains(filter.Where.Ename));

                if (!string.IsNullOrWhiteSpace(filter.Where.WorkNo))
                    query = query.Where(n => n.WorkNo != null && n.WorkNo.Contains(filter.Where.WorkNo));

                response = new SoaDataPageResponse<UsersView> { Count = query.Count() };

                query = !string.IsNullOrWhiteSpace(filter.OrderBy) ? query.SortBy(filter.OrderBy + " " + filter.SortCol) : query.OrderByDescending(n => n.CreateTime);

                response.Body = query.Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).ToList();
            }

            foreach (var user in response.Body)
            {
                user.RoleNames = string.Join(",", UserRolesDbAction.GetUserRoleNames(user.UserID));
            }

            return response;
        }
    }
}
