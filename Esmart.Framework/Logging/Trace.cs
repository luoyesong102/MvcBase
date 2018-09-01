using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Esmart.Framework.Caching;
using Esmart.Framework.Model;

namespace Esmart.Framework.Logging
{
    public class Trace : IDisposable
    {
        Stopwatch watch = null;
        LogTrace TraceModel = null;

        public Trace(string requestType, string message = null)
        {
            watch = new Stopwatch();
            watch.Start();
            TraceModel = new LogTrace();
            TraceModel.CreateTime = DateTime.Now;
            TraceModel.FromType = 1;
            TraceModel.RequestType = requestType;
            TraceModel.Message = message;
        }


        public Trace(string requestType, int fromType, string message = null)
        {
            watch = new Stopwatch();
            watch.Start();
            TraceModel = new LogTrace();
            TraceModel.CreateTime = DateTime.Now;
            TraceModel.FromType = fromType;
            TraceModel.RequestType = requestType;
            TraceModel.Message = message;
        }

        public void Dispose()
        {
            if (!ConstantDefine.NotUserLog)
            {
                try
                {
                    watch.Stop();
                    TraceModel.TotalTime = watch.ElapsedMilliseconds;
                    Esmart.Framework.DB.SQLManager.CreateSqlEngine("LogHelpDB").InsertObjectAsy(TraceModel);
                }
                catch
                {

                }
            }
        }
    }
}
