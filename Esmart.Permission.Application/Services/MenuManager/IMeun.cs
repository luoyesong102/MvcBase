using System.Collections.Generic;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.MenuManager
{
    [ServerAction(ServerType=typeof(MenuManager))]
    public interface IMeun
    {
        List<MenuResponse> GetMenus(int appId);


        int Add(MeunModel model);

        int Update(MeunModel model);

        /// <summary>
        /// 获取菜单下功能
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="navigationId"></param>
        /// <returns></returns>
        List<FunctionResponse> GetAllAndChoice(int appId, int navigationId);

        /// <summary>
        /// 新增关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdateNavigationFunction(NavigationFunctionRequest model);

        /// <summary>
        /// 删除by Id
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        int Delete(int menuId);

        /// <summary>
        /// 获取功能by navigationId
        /// </summary>
        /// <param name="navigationId"></param>
        /// <returns></returns>
        MeunModel GetMenusByNavigationId(int navigationId);

        /// <summary>
        /// 删除功能及子集
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        int DeleteAll(int menuId);

    }
}
