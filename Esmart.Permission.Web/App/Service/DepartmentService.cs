using System;
using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.DeparentManager;
using Esmart.Permission.Application.Models.Common;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;
using Esmart.Permission.Application;

namespace Esmart.Permission.Web
{
    public class DepartmentService
    {
        readonly IDepartment _departmentManager;

        public DepartmentService()
        {
            _departmentManager = new DepartmentManager();
        }

        public List<ZTreeNode> GetDepartments(int userId, int withUsers)
        {
            try
            {
                var result = _departmentManager.GetDepartmentsByUserId(userId, withUsers);

                var ztreeNodes = ToZTreeNode(result ?? Enumerable.Empty<DepartmentResponse>());

                if (withUsers > 0)
                {
                    if (CommonAction.IsSysAdmin(userId)) return ztreeNodes;

                    foreach (var node in ztreeNodes)
                    {
                        node.Type = "Root";
                        if (node.Children != null)
                        {
                            node.Children.RemoveAll(n => n.Type == "User" && n.Id == userId);
                        }
                    }
                }
                return ztreeNodes;
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public int AddDepartment(DepartmentRequest model)
        {
            try
            {
                return _departmentManager.Add(model);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public int UpdateDepartment(DepartmentRequest model)
        {
            try
            {
                return _departmentManager.Update(model);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public int DeleteDepartment(int departmentId)
        {
            try
            {
                return _departmentManager.Delete(departmentId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public SoaDataPageResponse<UsersView> GetUsersByDepartList(SoaDataPage<UserSearchModel> filter)
        {
            try
            {
                return _departmentManager.GetUsersByDepartList(filter);  
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public DepartmentResponse GetDepartmentResponse(int departmentId)
        {
            try
            {
                return _departmentManager.GetDepartmentResponse(departmentId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        private static List<ZTreeNode> ToZTreeNode(IEnumerable<DepartmentResponse> source)
        {
            var results = new List<ZTreeNode>(20);

            foreach (var item in source)
            {
                var ztreeNode = new ZTreeNode { Id = item.DeparentId, Name = item.Name, Type = "Department" };

                if (item.Users != null && item.Users.Any())
                {
                    ztreeNode.Children = new List<ZTreeNode>();

                    foreach (var user in item.Users)
                    {
                        ztreeNode.Children.Add(new ZTreeNode { Id = user.UserID, Name = user.UserName, Type = "User", IconSkin = "user" });
                    }
                }
                if (item.Children != null && item.Children.Any())
                {
                    if (ztreeNode.Children != null)
                        ztreeNode.Children.AddRange(ToZTreeNode(item.Children));
                    else ztreeNode.Children = ToZTreeNode(item.Children);
                }
                results.Add(ztreeNode);
            }
            return results;
        }
    }
}
