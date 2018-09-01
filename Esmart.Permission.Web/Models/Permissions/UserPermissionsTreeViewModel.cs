using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Esmart.Permission.Application;
using Esmart.Permission.Application.Models.ControlModel;

namespace Esmart.Permission.Web.Models.Permissions
{
    public class UserPermissionsTreeViewModel
    {
        public UserPermissionsTreeViewModel(int selUserId, int userId, int appId)
        {
            this.Init(selUserId,userId, appId);
        }

        public string TreeNodesJson { get; private set; }

        private void Init(int selUserId,int userId, int appId)
        {
            var response = new UserManager().GetUserMenuResponses(userId, appId,selUserId);
            var args = new UserPermissionsResponseModel { UserId=response.UserId,AllPermissions=response.AllPermissions,CurrentPermissions=response.CurrentPermissions};
            var treeNodes = GetTreeNodes(args);
            this.TreeNodesJson = JsonConvert.SerializeObject(treeNodes);
        }

        private static ICollection<UserPermissionTreeNode> GetTreeNodes(UserPermissionsResponseModel model)
        {
            ICollection<UserPermissionTreeNode> allTreeNodes = new List<UserPermissionTreeNode>();
            if (model.AllPermissions != null)
            {
                Map(model.AllPermissions, allTreeNodes);
            }

            ICollection<UserPermissionTreeNode> roleTreeNodes = new List<UserPermissionTreeNode>();
            if (model.CurrentPermissions != null)
            {
                Map(model.CurrentPermissions, roleTreeNodes);
            }

            var intersectNodes = allTreeNodes.Intersect(roleTreeNodes, new UserPermissionTreeNodeEqualityComparer()).ToList();
            Parallel.ForEach(intersectNodes, node =>
            {
                node.Checked = true;
                node.Open = true;
                node.Disabled = false;
            });

            var allExceptRoleNodes = allTreeNodes.Except(roleTreeNodes, new UserPermissionTreeNodeEqualityComparer()).ToList();
            Parallel.ForEach(allExceptRoleNodes, node =>
            {
                node.Checked = false;
                node.Open = false;
                node.Disabled = false;
            });

            var roleExceptAllNodes = roleTreeNodes.Except(allTreeNodes, new UserPermissionTreeNodeEqualityComparer()).ToList();
            Parallel.ForEach(roleExceptAllNodes, node =>
            {
                node.Checked = true;
                node.Open = true;
                node.Disabled = true;
            });

            var treeNodes = intersectNodes.Concat(allExceptRoleNodes).Concat(roleExceptAllNodes).ToList();

            return treeNodes;
        }

        private static void Map(IEnumerable<MenuResponse> permissions, ICollection<UserPermissionTreeNode> treeNodes)
        {
            foreach (var p in permissions)
            {
                treeNodes.Add(new UserPermissionTreeNode() { Id = p.Id.ToString(), ParentId = p.ParentId.ToString(), Name = p.Name, Category = PermissionCategory.Menu });
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

        private static void Map(string parentId, IEnumerable<FunctionSortInfo> functions, ICollection<UserPermissionTreeNode> treeNodes)
        {
            foreach (var f in functions)
            {
                treeNodes.Add(new UserPermissionTreeNode() { Id = string.Format("{0}-{1}", f.Id, parentId), ParentId = parentId, Name = f.Name, Category = PermissionCategory.Operation });
            }
        }
        class UserPermissionTreeNode
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

        class UserPermissionTreeNodeEqualityComparer : IEqualityComparer<UserPermissionTreeNode>
        {

            public bool Equals(UserPermissionTreeNode x, UserPermissionTreeNode y)
            {
                return x.Id == y.Id && x.Category == y.Category && x.ParentId == y.ParentId;
            }

            public int GetHashCode(UserPermissionTreeNode obj)
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

}