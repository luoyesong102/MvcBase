using System;
using System.Collections.Generic;
using System.Linq;
using Esmart.Permission.Application.Data;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.DbModel;
using Esmart.Framework.Model;

namespace Esmart.Permission.Application.Function
{
    public class FunctionManager : IFunction
    {
        /// <summary>
        /// 分页数据
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public SoaDataPageResponse<FunctionModel> GetList(SoaDataPage<FunctionQueryModelRequest> queryModel)
        {
            SoaDataPageResponse<FunctionModel> response = new SoaDataPageResponse<FunctionModel>();

            SoaDataPageResponse<Esmart_Sys_Functions> soa = FunctionDbAction.GetList(queryModel);
            List<FunctionModel> listFunction = new List<FunctionModel>();
            if (soa.Body.Any())
            {
                foreach (var li in soa.Body)
                {
                    listFunction.Add(new FunctionModel()
                    {
                        AppId = li.AppId,
                        CreateId = li.CreateId,
                        CreateTime = li.CreateTime,
                        FunctionId = li.FunctionId,
                        FunctionKey = li.FunctionKey,
                        FunctionName = li.FunctionName,
                        Remark = li.Remark
                    });
                }
            }
            response.Body = listFunction;
            response.Count = soa.Count;
            return response;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public bool Add(FunctionModel queryModel)
        {
            Esmart_Sys_Functions functions = new Esmart_Sys_Functions() { AppId = queryModel.AppId, CreateId = queryModel.CreateId, CreateTime = DateTime.Now, FunctionKey = queryModel.FunctionKey, FunctionName = queryModel.FunctionName, Remark = queryModel.Remark };
            return FunctionDbAction.Add(functions);
        }

        /// <summary>
        /// 删除 
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        public bool Del(int functionId)
        {
            return FunctionDbAction.Del(functionId);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public bool Update(FunctionModel queryModel)
        {
            Esmart_Sys_Functions functions = new Esmart_Sys_Functions() { AppId = queryModel.AppId, CreateId = queryModel.CreateId, FunctionKey = queryModel.FunctionKey, FunctionName = queryModel.FunctionName, Remark = queryModel.Remark, FunctionId = queryModel.FunctionId };
            return FunctionDbAction.Update(functions);
        }

        /// <summary>
        ///  获取一个实例by functionId
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        public FunctionModel GetFunctionById(int functionId)
        {
            FunctionModel functionModel = new FunctionModel();

            var dbFunction = FunctionDbAction.GetFunctionById(functionId);
            if (dbFunction != null)
            {
                functionModel.FunctionId = dbFunction.FunctionId;
                functionModel.AppId = dbFunction.AppId;
                functionModel.CreateId = dbFunction.CreateId;
                functionModel.CreateTime = dbFunction.CreateTime;
                functionModel.FunctionKey = dbFunction.FunctionKey;
                functionModel.FunctionName = dbFunction.FunctionName;
                functionModel.Remark = dbFunction.Remark;
            }
            return functionModel;
        }
    }
}
