using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Esmart.Permission.Application;
using Esmart.Permission.Application.Models.ControlModel;

namespace Esmart.Permission.Web.Models.Permissions
{
    public class RolePermissionsTreeViewModel
    {
        public RolePermissionsTreeViewModel(int roleId, int userId, int appId)
        {
            this.Init(roleId, userId, appId);
        }

        public string TreeNodesJson { get; private set; }

        private void Init(int roleId, int userId, int appId)
        {
            var response = new UserManager().GetMenuResponses(userId, appId, roleId);
            var treeNodes = GetTreeNodes(response);
            this.TreeNodesJson = JsonConvert.SerializeObject(treeNodes);
        }

        private static ICollection<RolePermissionTreeNode> GetTreeNodes(RolePermissionsResponseModel model)
        {
            ICollection<RolePermissionTreeNode> allTreeNodes = new List<RolePermissionTreeNode>();
            if (model.AllPermissions != null)
            {
                Map(model.AllPermissions, allTreeNodes);
            }

            ICollection<RolePermissionTreeNode> roleTreeNodes = new List<RolePermissionTreeNode>();
            if (model.CurrentPermissions != null)
            {
                Map(model.CurrentPermissions, roleTreeNodes);
            }

            var intersectNodes = allTreeNodes.Intersect(roleTreeNodes, new RolePermissionTreeNodeEqualityComparer()).ToList();
            Parallel.ForEach(intersectNodes, node =>
            {
                node.Checked = true;
                node.Open = true;
                node.Disabled = false;
            });

            var allExceptRoleNodes = allTreeNodes.Except(roleTreeNodes, new RolePermissionTreeNodeEqualityComparer()).ToList();
            Parallel.ForEach(allExceptRoleNodes, node =>
            {
                node.Checked = false;
                node.Open = false;
                node.Disabled = false;
            });

            var roleExceptAllNodes = roleTreeNodes.Except(allTreeNodes, new RolePermissionTreeNodeEqualityComparer()).ToList();
            Parallel.ForEach(roleExceptAllNodes, node =>
            {
                node.Checked = true;
                node.Open = true;
                node.Disabled = true;
            });

            var treeNodes = intersectNodes.Concat(allExceptRoleNodes).Concat(roleExceptAllNodes).ToList();

            return treeNodes;
        }

        private static void Map(IEnumerable<MenuResponse> permissions, ICollection<RolePermissionTreeNode> treeNodes)
        {
            foreach (var p in permissions)
            {
                treeNodes.Add(new RolePermissionTreeNode() { Id = p.Id.ToString(), ParentId = p.ParentId.ToString(), Name = p.Name, Category = PermissionCategory.Menu });
                if (p.Functions != null)
                {
                    Map(p.Id.ToString(), p.Functions, treeNodes);
                }
                if (p.Children != null)
                {
                    Map(p.Children, treeNodes);
                }
            }
        }

        private static void Map(string parentId, IEnumerable<FunctionSortInfo> functions, ICollection<RolePermissionTreeNode> treeNodes)
        {
            foreach (var f in functions)
            {
                treeNodes.Add(new RolePermissionTreeNode() { Id = string.Format("{0}-{1}", f.Id, parentId), ParentId = parentId, Name = f.Name, Category = PermissionCategory.Operation });
            }
        }

        class RolePermissionTreeNode
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("pId")]
            public string ParentId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("category")]
            public PermissionCategory Category { get; set; }

            [JsonProperty("open")]
            public bool? Open { get; set; }

            [JsonProperty("checked")]
            public bool? Checked { get; set; }

            [JsonProperty("chkDisabled")]
            public bool? Disabled { get; set; }
        }

        class RolePermissionTreeNodeEqualityComparer : IEqualityComparer<RolePermissionTreeNode>
        {

            public bool Equals(RolePermissionTreeNode x, RolePermissionTreeNode y)
            {
                return x.Id == y.Id && x.Category == y.Category && x.ParentId == y.ParentId;
            }

            public int GetHashCode(RolePermissionTreeNode obj)
            {
                if (obj == null || string.IsNullOrWhiteSpace(obj.Name))
                {
                    return 0;
                }
                else
                {
                    return obj.Name.GetHashCode();
                }
            }
        }
    }

    /// <summary>
    /// 权限类别
    /// </summary>
    public enum PermissionCategory
    {
        /// <summary>
        /// 菜单权限
        /// </summary>
        Menu = 0,

        /// <summary>
        /// 操作权限
        /// </summary>
        Operation = 1
    }
}