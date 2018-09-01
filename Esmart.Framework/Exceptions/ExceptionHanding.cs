using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Eureka.IAI.Solution.Core.Common
{
    public static class ExceptionHanding
    {
        /// <summary>
        /// 记录常规的exception错误
        /// </summary>
        /// <param name="exception"></param>
        public static void HandleException(this Exception exception)
        {
            Task.Factory.StartNew(() =>
                {
                    var message = "";
                    message += "发生时间：" + DateTime.Now + Environment.NewLine;
                    message += "异常堆栈：" + exception.StackTrace.Trim() + Environment.NewLine;
                    message += "异常消息：" + exception.Message + Environment.NewLine;
                    message += "异常来源：" + exception.Source.Trim();
                    WriteLog(message);
                });
        }

        public static void Log(this string message)
        {
            WriteLog(message);
        }

        private static void WriteLog(string msg)
        {
            try
            {
                string rootPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;//获取或设置包含该应用程序的目录的名称。

                //创建后台服务错误日志文件夹
                var strPath = ConfigurationManager.AppSettings["Logs"];
                if (string.IsNullOrEmpty(strPath))
                {
                    strPath = rootPath + "\\Logs";
                }

                if (Directory.Exists(strPath) == false)
                {
                    Directory.CreateDirectory(strPath);
                }

                //创建日期文件
                strPath = strPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                var fileWriter = new StreamWriter(strPath, true);
                var sb = new StringBuilder();
                sb.Append("======================================================");
                sb.Append(Environment.NewLine);
                sb.Append(msg);
                sb.Append(Environment.NewLine);
                sb.Append("======================================================");
                sb.Append(Environment.NewLine);
                fileWriter.WriteLine(sb.ToString());
                fileWriter.Close(); //关闭StreamWriter对象
            }
            catch (Exception exception)
            {
                //("写错误日志时出现问题，请与管理员联系！ 原错误:" + strMatter + "写日志错误:" + ex.Message.ToString());
                //throw new Exception(exception.Message);
            }
        }
    }
}
