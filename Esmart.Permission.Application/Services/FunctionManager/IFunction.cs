using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Function
{

    [ServerAction(ServerType = typeof(FunctionManager))]
    public interface IFunction
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        SoaDataPageResponse<FunctionModel> GetList(SoaDataPage<FunctionQueryModelRequest> queryModel);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        bool Add(FunctionModel queryModel);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        bool Del(int functionId);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        bool Update(FunctionModel queryModel);

        /// <summary>
        /// 获取一个实例by functionId
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        FunctionModel GetFunctionById(int functionId);
    }
}
