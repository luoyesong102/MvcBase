using System;
using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Data.Permission;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.MenuManager
{
    public class MenuManager : IMeun
    {
        private List<Esmart_Sys_Navigations> listData;
        private List<Esmart_Sys_Functions> listFunctions;

        public List<MenuResponse> GetMenus(int appId)
        {
            listData = Data.MenuManager.GetAllMenus(appId);

            listFunctions = FunctionDbAction.GetAllFunctions(appId);

            var listMenuFunctions = FunctionDbAction.GetMenusFunctions(listData.ConvertAll(a => a.NavigationId));

            var firstMenus = listData.FindAll(a => a.ParentID == 0).ConvertAll(a => new MenuResponse() { Iconurl = a.Iconurl, Id = a.NavigationId, InClassName = a.InClassName, OutClassName = a.OutClassName, Url = a.Url, Name = a.Title });

            SetList(listMenuFunctions, firstMenus);

            return firstMenus;
        }

        private void SetList(List<Esmart_Sys_Navigation_Function> listMenuFunctions, List<MenuResponse> list)
        {
            if (list == null)
            {
                return;
            }
            if (list.Count == 0)
            {
                return;
            }
            foreach (var menu in list)
            {
                var functions = listMenuFunctions.FindAll(a => a.NavigationId == menu.Id).ConvertAll(a => a.FunctionId);

                menu.Functions = listFunctions.FindAll(a => functions.Contains(a.FunctionId)).ConvertAll(a => new FunctionSortInfo() { Id = a.FunctionId, Name = a.FunctionName, Key = a.FunctionKey });

                var childs = listData.FindAll(a => a.ParentID == menu.Id);

                if (childs.Count > 0)
                {
                    menu.Children = childs.ConvertAll(a => new MenuResponse() { Iconurl = a.Iconurl, Id = a.NavigationId, Name = a.Title, InClassName = a.InClassName, OutClassName = a.OutClassName, Url = a.Url });

                    SetList(listMenuFunctions, menu.Children);
                }
            }
        }

        /// <summary>
        /// 添加一个菜单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(MeunModel model)
        {
            Esmart_Sys_Navigations DbModel = new Esmart_Sys_Navigations() { AppId = model.AppId, CreateId = model.CreateId, CreateTime = model.CreateTime, Iconurl = model.Iconurl, InClassName = model.InClassName, NavigationId = model.Id, OutClassName = model.OutClassName, ParentID = model.ParentID, SortNo = model.SortNo, Title = model.Title, Url = model.Url };

            int code = Data.MenuManager.Add(DbModel);

            return code;
        }

        public int Update(MeunModel model)
        {
            Esmart_Sys_Navigations dbModel = new Esmart_Sys_Navigations() { AppId = model.AppId, Iconurl = model.Iconurl, InClassName = model.InClassName, NavigationId = model.Id, OutClassName = model.OutClassName, ParentID = model.ParentID, SortNo = model.SortNo, Title = model.Title, Url = model.Url };

            var updateModel = Data.MenuManager.GetModel(model.Id);

            if (updateModel == null)
            {
                throw new TpoBaseException("this  data  is  not existes");
            }
            dbModel.CreateId = updateModel.CreateId;
            dbModel.CreateTime = updateModel.CreateTime;

            int code = Data.MenuManager.Update(dbModel);

            LogManagerDb.Log(model.CreateId, model.CreateTime, "用户" + model.CreateId + "修改了菜单，菜单名字是：" + model.Title, "menuupdate");

            return code;

        }

        public List<FunctionResponse> GetAllAndChoice(int appId, int navigationId)
        {
            var response = new List<FunctionResponse>();

            var listFunction = FunctionDbAction.GetAllFunctions(appId);

            var listNavigation = NavigationFunctionDbAction.GetListByNavigationId(navigationId);

            var isNull = listNavigation != null && listNavigation.Any();

            if (listFunction == null || !listFunction.Any()) return response;

            foreach (var f in listFunction)
            {
                var function = new FunctionResponse
                {
                    Id = f.FunctionId,
                    Name = f.FunctionName
                };
                if (isNull && listNavigation.Any(m => m.FunctionId == f.FunctionId))
                {
                    function.IsChoice = true;
                }
                else
                {
                    function.IsChoice = false;
                }
                response.Add(function);
            }
            return response;
        }

        public bool UpdateNavigationFunction(NavigationFunctionRequest model)
        {
            if (model != null)
            {
                List<Esmart_Sys_Navigation_Function> list = new List<Esmart_Sys_Navigation_Function>();
                if (model.ListFunctionId != null && model.ListFunctionId.Any())
                {
                    foreach (var m in model.ListFunctionId)
                    {
                        list.Add(new Esmart_Sys_Navigation_Function() { CreateId = model.CreatId, CreateTime = DateTime.Now, FunctionId = m, NavigationId = model.NavigationId });
                    }
                }
                return NavigationFunctionDbAction.Update(list, model.NavigationId);
            }
            return false;
        }


        public int Delete(int menuId)
        {
            var dbModel = Data.MenuManager.GetModel(menuId);

            if (dbModel == null)
            {
                throw new TpoBaseException("this  data  is  not existes");
            }

            int code = Data.MenuManager.Delete(dbModel);

            return code;
        }


        public MeunModel GetMenusByNavigationId(int navigationId)
        {
            MeunModel model = new MeunModel();

            var tpoSysNavigations = Data.MenuManager.GetModel(navigationId);
            if (tpoSysNavigations != null)
            {
                model.AppId = tpoSysNavigations.AppId;
                model.CreateId = tpoSysNavigations.CreateId;
                model.CreateTime = tpoSysNavigations.CreateTime;
                model.Iconurl = tpoSysNavigations.Iconurl;
                model.Id = tpoSysNavigations.NavigationId;
                model.InClassName = tpoSysNavigations.InClassName;
                model.OutClassName = tpoSysNavigations.OutClassName;
                model.ParentID = tpoSysNavigations.ParentID;
                model.SortNo = tpoSysNavigations.SortNo;
                model.Title = tpoSysNavigations.Title;
                model.Url = tpoSysNavigations.Url;
            }
            return model;
        }

        public int DeleteAll(int menuId)
        {
            var listId = new List<int>() { menuId };
            var list = GetList(menuId);
            if (list != null && list.Any())
            {
                listId.AddRange(list.Select(li => li.NavigationId));
            }
            var returnInt = Data.MenuManager.DelByList(listId);
            CommonAction.ClearCache();
            return returnInt;
        }

        public List<Esmart_Sys_Navigations> GetList(int parentId)
        {
            List<Esmart_Sys_Navigations> delList = new List<Esmart_Sys_Navigations>(parentId);

            List<Esmart_Sys_Navigations> list = Data.MenuManager.GetListByParentId(parentId);
            if (list != null && list.Any())
            {
                foreach (var li in list)
                {
                    delList.Add(new Esmart_Sys_Navigations() { NavigationId = li.NavigationId });
                    GetList(li.NavigationId);
                }
            }
            return delList;
        }
    }
}
