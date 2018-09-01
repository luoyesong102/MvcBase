using Esmart.Permission.Application.AppManager;
using Esmart.Permission.Application.Models.ControlModel;
using System;
using System.Collections.Generic;
using Esmart.Framework.Model;

namespace Esmart.Permission.Web
{
    public class AppService
    {
        IApp _appManager;

        public AppService()
        {
            _appManager = new AppManager();
        }

        public List<AppShortInfo> GetAppList()
        {
            try
            {
                return _appManager.GetAppList();
            }
            catch (Exception ex)
            {
                throw new TpoBaseException(ex.Message);
            }
        }
    }
}
