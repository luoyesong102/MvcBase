using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Reflection;
using System.ServiceModel.Description;

namespace Esmart.Framework.Utilities
{
    /// <summary>
    /// 动态调用WCF的工具类库
    /// </summary>
    public class WCFHelper
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="I"></typeparam>
        /// <param name="name"></param>
        public static void StartService<T, I>(string name,string uri)
        {
            ServiceHost ControHost = new ServiceHost(typeof(T), new Uri[] { new Uri(uri) });
            ServiceMetadataBehavior behavior = ControHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            {
                if (behavior == null)
                {
                    behavior = new ServiceMetadataBehavior();
                    ControHost.Description.Behaviors.Add(behavior);
                }
            }
            ControHost.AddServiceEndpoint(typeof(I), CreateBinding("nettcpbinding"), "");
            ControHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            ControHost.Opened += delegate
            {
                Console.WriteLine(name + " has begun to listen  ");
            };

            ControHost.Open();

            Console.WriteLine(name + " start..");
        }

        #region Wcf服务工厂

        public static void DoWcfInvoke<T>(string url, Action<T> Invoke)
        {
            ChannelFactory<T> factory =null;
            try {
                        if (string.IsNullOrEmpty(url)) throw new NotSupportedException("this url isn`t Null or Empty!");
                EndpointAddress address = new EndpointAddress(url);
                Binding binding = CreateBinding("nettcpbinding");
               factory = new ChannelFactory<T>(binding, address);
                T client =  factory.CreateChannel();
                Invoke.Invoke(client);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                factory.Close();
                factory.Abort();
            }
        }
        public  static T CreateWCFServiceByURL<T>(string url)
        {
            return CreateWCFServiceByURL<T>(url, "nettcpbinding");
        }


        public static T CreateWCFServiceByURL<T>(string url, string bing)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) throw new NotSupportedException("this url isn`t Null or Empty!");
                EndpointAddress address = new EndpointAddress(url);
                Binding binding = CreateBinding(bing);
                ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
                return factory.CreateChannel();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 创建传输协议
        /// <summary>
        /// 创建传输协议
        /// </summary>
        /// <param name="binding">传输协议名称</param>
        /// <returns></returns>
        private static Binding CreateBinding(string binding)
        {
            Binding bindinginstance = null;
            if (binding.ToLower() == "basichttpbinding")
            {
                BasicHttpBinding ws = new BasicHttpBinding();
                ws.MaxBufferSize = 2147483647;
                ws.MaxBufferPoolSize = 2147483647;
                ws.MaxReceivedMessageSize = 2147483647;
                ws.ReaderQuotas.MaxStringContentLength = 2147483647;
                ws.CloseTimeout = new TimeSpan(0, 10, 0);
                ws.OpenTimeout = new TimeSpan(0, 10, 0);
                ws.ReceiveTimeout = new TimeSpan(0, 10, 0);
                ws.SendTimeout = new TimeSpan(0, 10, 0);

                bindinginstance = ws;
            }
            else if (binding.ToLower() == "netnamedpipebinding")
            {
                NetNamedPipeBinding ws = new NetNamedPipeBinding();
                ws.MaxReceivedMessageSize = 2147483647;
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "netpeertcpbinding")
            {
                NetPeerTcpBinding ws = new NetPeerTcpBinding();
                ws.MaxReceivedMessageSize = 2147483647;
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "nettcpbinding")
            {
                NetTcpBinding ws = new NetTcpBinding();
                ws.MaxReceivedMessageSize = 2147483647;
                ws.Security.Mode = SecurityMode.None;
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "wsdualhttpbinding")
            {
                WSDualHttpBinding ws = new WSDualHttpBinding();
                ws.MaxReceivedMessageSize = 2147483647;

                bindinginstance = ws;
            }
            else if (binding.ToLower() == "webhttpbinding")
            {
                //WebHttpBinding ws = new WebHttpBinding();
                //ws.MaxReceivedMessageSize = 65535000;
                //bindinginstance = ws;
            }
            else if (binding.ToLower() == "wsfederationhttpbinding")
            {
                WSFederationHttpBinding ws = new WSFederationHttpBinding();
                ws.MaxReceivedMessageSize = 2147483647;
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "wshttpbinding")
            {
                WSHttpBinding ws = new WSHttpBinding(SecurityMode.None);
                ws.MaxReceivedMessageSize = 2147483647;
                ws.Security.Message.ClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
                ws.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
                bindinginstance = ws;
            }
            return bindinginstance;

        }
        #endregion

    }
}