using System;
using System.Collections.Generic;
using Esmart.Permission.Application.MenuManager;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.Common;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web
{
    public class MenuService
    {
        IMeun _menuManager;

        public MenuService()
        {
            _menuManager = new MenuManager();
        }

        public List<ZTreeNodeJson> GetNavigationTreeData(int AppId)
        {
            try
            {
                var data = _menuManager.GetMenus(AppId);
                var result = ConvertToTreeNode(data);
                return result;
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        private static List<ZTreeNodeJson> ConvertToTreeNode(List<MenuResponse> menu)
        {
            var list = new List<ZTreeNodeJson>();

            foreach (var item in menu)
            {
                var itemM = new ZTreeNodeJson();
                itemM.id = item.Id;
                itemM.name = item.Name;
                itemM.children = SetList(item.Children);
                list.Add(itemM);
            }
            return list;

        }

        /// <summary>
        /// 转换树形数据
        /// </summary>
        /// <param name="list">子节点集</param>
        /// <returns></returns>
        private static List<ZTreeNodeJson> SetList(List<MenuResponse> list)
        {
            List<ZTreeNodeJson> treelist = new List<ZTreeNodeJson>();
            if (list != null)
            {
                foreach (var menu in list)
                {
                    var item = new ZTreeNodeJson();
                    item.id = menu.Id;
                    item.name = menu.Name;
                    item.children = SetList(menu.Children);
                    treelist.Add(item);
                }
            }
            return treelist;
        }

        public int NavigationSave(MeunModel model)
        {
            try
            {
                if (model.Id == 0)
                {
                    return _menuManager.Add(model);
                }
                return _menuManager.Update(model);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public List<FunctionResponse> GetAllAndChoice(int appId, int navigationId)
        {
            try
            {
                return _menuManager.GetAllAndChoice(appId, navigationId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool NavigationFunction(NavigationFunctionRequest model)
        {
            try
            {
                return _menuManager.UpdateNavigationFunction(model);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public MeunModel GetMenusByNavigationId(int navigationId)
        {
            try
            {
                return _menuManager.GetMenusByNavigationId(navigationId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public int DeleteAll(int menuId)
        {
            try
            {
                return _menuManager.DeleteAll(menuId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }
    }
}
