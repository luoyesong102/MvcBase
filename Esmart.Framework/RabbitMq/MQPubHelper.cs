using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using EasyNetQ;
using EasyNetQ.Topology;
using EasyNetQ.Loggers;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
namespace Esmart.Framework.RabbitMq
{
  
    public class MQPubHelper:IDisposable
    {
       public static IBus bus;
       readonly string logtype = ConfigurationManager.AppSettings["LogType"].ToString();
       public IEasyNetQLogger ilog;
       /// <summary>
       /// 消费者接收到消息时出发
       /// </summary>
      // public event EventHandler<ConsumerReceiveMessageEventArgs> RabbitReceivingMessage;
       private MQPubHelper()
        {
           
        }
        public static MQPubHelper Instance
        {
            get
            {
                
                return SingletonProxy<MQPubHelper>.Create(() => new MQPubHelper());
            }
        }
        /// <summary>
        /// 创建连接消息中心，可配置configuration的参数带输出参数，也可回传发布消息的接口，如果消息未达到，或者达到队列发现无消费者，都可以控制（无消费者的情况目前已不进行支持）
        /// </summary>
        private void CreateBus()
        {
             string connString = ConfigurationManager.AppSettings["RabbitAddress"].ToString();
             bus = RabbitHutch.CreateBus(connString);
             bus.Advanced.MessageReturned += returnmessage;
        }
        /// <summary>
        /// 创建连接消息中心，可配置configuration的参数带输出参数，也可回传发布消息的接口，如果消息未达到，或者达到队列发现无消费者，都可以控制（无消费者的情况目前已不进行支持）
        /// </summary>
        public  void CreateBusAdvanced()
        {
            var logger = LogFactory.GetLogType();//new ConsoleLogger(); // implements IEasyNetQLogger
            string connString = ConfigurationManager.AppSettings["RabbitAddress"].ToString();
            bus = RabbitHutch.CreateBus(connString, x => x.Register<IEasyNetQLogger>(_ => logger));
            bus.Advanced.MessageReturned += returnmessage;
          
        }
       
       private void CheckConnected()
       {
            if (bus!=null)
                {
                    if(bus.IsConnected)
                    {

                    }
                }
                else
                {
                    CreateBusAdvanced();
                }
       }

       /// <summary>
       /// 直接给队列发送消息
       /// </summary>
       public void PublishToQueue<T>(MyMessage<T> msg)
       {

           try
           {
               CheckConnected();
               bus.SendAsync<MyMessage<T>>(msg.MessageQueue, msg).ContinueWith(task =>
               {

                   if (task.IsCompleted)
                   {
                       //安全抵达队列publisherConfirms参数
                   }
                   if (task.IsFaulted)
                   {
                       Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + task.Exception, (int)LogType.Rabbitmq);
                   }
               }); 


           }
           catch (Exception ex)
           {
               Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);
           }

           //bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
       }
       /// <summary>
       /// 高级-发送消息(带交换和路由),自定义交换机，路由，一般发送不创建交换机，passive设置为true 返回消息mandatory为TRUE
       /// </summary>
       public void PublishByRetrun<T>(MyMessage<T> msg)
       {

           try
           {
               CheckConnected();
               IExchange exchange = null;
               try
               {
                   exchange = bus.Advanced.ExchangeDeclare(msg.MessageExchange, msg.MessageExchangeType, false);//true不允许创建不存在的队列passive

               }
               catch (Exception ex)
               {
                   Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);
               }

               var message = new Message<MyMessage<T>>(msg);
               bus.Advanced.PublishAsync<MyMessage<T>>(exchange, msg.MessageRouter, true, message);//交换机和路由 ,mandatory如果发现抵达不了的话false会自己销毁，Ture会返回给生产者


           }
           catch (Exception ex)
           {
               Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);

           }

           //bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
       }
       public void PublishByRetrunMore<T>(MyMessage<T> msg)
       {

           try
           {
               CheckConnected();
               IExchange exchange = null;
               try
               {
                   exchange = bus.Advanced.ExchangeDeclare(msg.MessageExchange, msg.MessageExchangeType, false);//true不允许创建不存在的队列passive

               }
               catch (Exception ex)
               {
                   Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);
               }

               var message = new Message<MyMessage<T>>(msg);
               MessageProperties mesageprop = new MessageProperties();
               mesageprop.Priority = 0;
               mesageprop.DeliveryMode = 2;
               var a = Newtonsoft.Json.JsonConvert.SerializeObject(message); ;//这个才是具体的发送内容
               bus.Advanced.PublishAsync(exchange, msg.MessageRouter, true, mesageprop, Encoding.UTF8.GetBytes(a));//交换机和路由 ,mandatory如果发现抵达不了的话false会自己销毁，Ture会返回给生产者


           }
           catch (Exception ex)
           {
               Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);

           }

          // bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
       }
       private void returnmessage(object sender, MessageReturnedEventArgs age)
       {
           Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + JsonConvert.SerializeObject(age.MessageBody) + "\r\n" + JsonConvert.SerializeObject(age.MessageReturnedInfo), (int)LogType.Rabbitmq);
            bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
           //是否经过了交换和路由，却找不到队列。
       }
    
      /// <summary>
        /// 普通发布-默认交换机topic,路由是#,交换机自动生成
      /// </summary>
      /// <param name="msg"></param>
        private  void Publish<T>(MyMessage<T> msg)
        {
       
            try
            {
                CheckConnected();
                bus.Publish(msg);//默认路由是#,交换机自动生成
            }
            catch (Exception ex)
            {
                Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message +  "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);
                //处理连接消息服务器异常 
            }

           // bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
        }
        /// <summary>
        /// 普通发布-默认交换机topic,路由自定义,交换机自动生成
        /// </summary>
        /// <param name="msg"></param>
        private void PublishTopic<T>(MyMessage<T> msg)
        {
           
            try
            {
                CheckConnected();
                bus.Publish<MyMessage<T>>(msg, msg.MessageRouter);
         
               
            }
            catch (Exception ex)
            {
                //处理连接消息服务器异常 
                Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);

            }

            //bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
        }

        /// <summary>
        /// 高级-发送消息,自定义交换机，路由，一般发送不创建交换机，passive设置为true 返回消息mandatory为false
        /// </summary>
        private  void PublishNoRetrun<T>(MyMessage<T> msg)
        {

            try
            {
                CheckConnected();
                IExchange exchange = null;
                try
                {
                    exchange = bus.Advanced.ExchangeDeclare(msg.MessageExchange, ExchangeType.Topic, true);//true不允许创建不存在的队列

                }
                catch (Exception ex)
                {
                    Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);
                }

                var message = new Message<MyMessage<T>>(msg);
                bus.Advanced.PublishAsync<MyMessage<T>>(exchange, msg.MessageRouter, false, message);//交换机和路由 ,mandatory如果发现抵达不了的话false会自己销毁，Ture会返回给生产者


            }
            catch (Exception ex)
            {
                Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);

            }

            // bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
        }

        /// <summary>
      /// 普通-发送消息并确认Publisher confirm 机制是用来确认 message 的可靠投递
        /// </summary>
        private  void PublishByConfirm<T>(MyMessage<T> msg)
        {
           
            try
            {
                CheckConnected();
                bus.PublishAsync(msg).ContinueWith(task =>
                {
                   
                    if (task.IsCompleted)
                    {
                      
                    }
                    if (task.IsFaulted)
                    {
                        Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + task.Exception , (int)LogType.Rabbitmq);
                    }
                });

            }
            catch (Exception ex)
            {
                Log.WriteLog("Publish: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);

            }

           // bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
        }
        public void Dispose()
        {
            try
            {
                if (bus != null)
                {

                    bus.Dispose();
                }
            }
            catch { }
            finally
            {
                bus = null;
            }
        }
    }
}
