using System;
using Esmart.Permission.Application.Function;
using Esmart.Permission.Application.Models.ControlModel;
using Esmart.Permission.Application.Models.Permissions;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web
{
    public class FunctionService
    {
        IFunction _functionManager;

        public FunctionService()
        {
            _functionManager = new FunctionManager();
        }

        public SoaDataPageResponse<FunctionModel> GetFunctionsOfApp(SoaDataPage<Function> soa)
        {
            SoaDataPage<FunctionQueryModelRequest> queryModel = new SoaDataPage<FunctionQueryModelRequest>()
            {
                PageIndex = soa.PageIndex,
                PageSize = soa.PageSize,
                Where = new FunctionQueryModelRequest()
                {
                    AppId = soa.Where.AppId
                }
            };

            try
            {
                return _functionManager.GetList(queryModel);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool SaveFunction(FunctionModel model)
        {
            try
            {
                if (model.FunctionId == 0)
                {
                    return _functionManager.Add(model);
                }
                return _functionManager.Update(model);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public bool DeleteFunction(int functionId)
        {
            try
            {
                return _functionManager.Del(functionId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }

        public FunctionModel GetFunction(int functionId)
        {
            try
            {
                return _functionManager.GetFunctionById(functionId);
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }
    }
}
