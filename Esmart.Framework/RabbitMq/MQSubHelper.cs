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
    public class MQSubHelper : IDisposable
    {
        public static IBus bus;
       
        /// <summary>
        /// 消费者接收到消息时出发
        /// </summary>
        // public event EventHandler<ConsumerReceiveMessageEventArgs> RabbitReceivingMessage;
        private MQSubHelper()
        {

        }
        public static MQSubHelper Instance
        {
            get
            {

                return SingletonProxy<MQSubHelper>.Create(() => new MQSubHelper());
            }
        }
        /// <summary>
        /// 创建连接消息中心，可配置configuration的参数带输出参数，也可回传发布消息的接口，如果消息未达到，或者达到队列发现无消费者，都可以控制（无消费者的情况目前已不进行支持）
        /// </summary>
        private void CreateBus()
        {
            string connString = ConfigurationManager.AppSettings["RabbitAddress"].ToString();
            bus = RabbitHutch.CreateBus(connString);
        }
        private void CreateBusByConmmit()
        {
            string connString = ConfigurationManager.AppSettings["RabbitAddress"].ToString();
            ConnectionConfiguration configuration = new ConnectionConfiguration();
            bus = RabbitHutch.CreateBus( );
        }
        /// <summary>
        /// 创建连接消息中心，可配置configuration的参数带输出参数，也可回传发布消息的接口，如果消息未达到，或者达到队列发现无消费者，都可以控制（无消费者的情况目前已不进行支持）
        /// </summary>
        public void CreateBusAdvanced()
        {
            var logger = LogFactory.GetLogType();//new ConsoleLogger(); // implements IEasyNetQLogger
            string connString = ConfigurationManager.AppSettings["RabbitAddress"].ToString();
            bus = RabbitHutch.CreateBus(connString, x => x.Register<IEasyNetQLogger>(_ => logger));
        }
        private void CheckConnected()
        {
          
            if (bus != null)
            {
                if (bus.IsConnected)
                {

                }
            }
            else
            {
                CreateBusAdvanced();
            }
        }


        /// <summary>
        /// 只有队列的直接订阅
        /// </summary>
        /// <param name="Queue">队列名</param>
        /// 
        public void SubscribeByQueue<T>(string Queue, Action<MyMessage<T>> agr)
        {
            try
            {
                CheckConnected();
                bus.Receive<MyMessage<T>>(Queue, message =>
                {

                    agr(message);
                }
                    );
            }

            catch (Exception ex)
            {
                Log.WriteLog("Receive: 时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " 错误信息：" + ex.Message + "\r\n" + ex.StackTrace, (int)LogType.Rabbitmq);

            }

        }
       

        /// <summary>
        /// 高级消费可以自定义交换机及队列
        /// </summary>
        /// <param name="queuestr">队列</param>
        /// <param name="exchangestr">交换机</param>
        /// <param name="routkey">路由</param>
        /// <param name="exchangtype">交换机类型</param>
        public void SubscribeBySelf(string queuestr, string exchangestr, string routkey, string exchangtype,Action<string> messages)
        {
            CheckConnected();
            var queue = bus.Advanced.QueueDeclare(queuestr);
            var exchange = bus.Advanced.ExchangeDeclare(exchangestr, exchangtype);
            var binding = bus.Advanced.Bind(exchange, queue, routkey);


            bus.Advanced.Consume(queue, (body, properties, info) => Task.Factory.StartNew(() =>
            {

                var message = Encoding.UTF8.GetString(body);
                messages(message);
                //Console.WriteLine("Got message: '{0}'", message);
            }));
        }
        /// <summary>
        /// //高级可以自定义交换机及队列
        /// </summary>
        public void SubscribeByMessage<T>(string queuestr, string exchangestr, string routkey, string exchangtype, Action<MyMessage<T>> messages)
        {
            CheckConnected();
            var queue = bus.Advanced.QueueDeclare(queuestr);
            var exchange = bus.Advanced.ExchangeDeclare(exchangestr, exchangtype);
            var binding = bus.Advanced.Bind(exchange, queue, routkey);
            bus.Advanced.Consume<MyMessage<T>>(queue, (message, info) =>
            {
               // messages.Invoke(message.Body);
               messages(message.Body);
                //Thread.Sleep(10000);
                //Console.WriteLine("Got message: '{0}'", message.Body.MessageBody);
                //Console.WriteLine("Got message: '{0}'", info.ConsumerTag);
            });
        }
        /// <summary>
        /// 普通订阅，交换机自动生成且只能是topic,或者交换机和队列映射实体对象上
        /// </summary>
        private void Subscribe()
        {
            CheckConnected();
            bus.Subscribe<MyMessage<string>>(string.Empty, msg1 => Console.WriteLine(msg1.MessageBody));


        }
        /// <summary>
        /// 普通订阅,带topic的匹配，交换机，队列拼接
        /// </summary>
        /// <param name="substring">交换机，队列拼接子部分</param>
        /// <param name="topic">路由值</param>
        private void SubscribeByTopic(string substring, string topic)//普通PUB,带topic的匹配
        {
            CheckConnected();
            bus.Subscribe<MyMessage<string>>(substring, message => Console.WriteLine("MyMessage<string>: {0}", message.MessageBody), x => x.WithTopic(topic));
        }
        /// <summary>
        /// 普通订阅,带topic的匹配，交换机，队列拼接
        /// </summary>
        /// <param name="substring">交换机，队列拼接子部分</param>
        /// <param name="topic">路由值</param>
        private void SubscribeByTopicAsync(string substring, string topic)//普通PUB,带topic的匹配SubscribeAsync
        {
            CheckConnected();
            bus.SubscribeAsync<MyMessage<string>>(substring, (message) => Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Got message: '{0}'", message.MessageBody);
            })
            , x => x.WithTopic(topic));
        }
      
        /// <summary>
        /// 只有队列的直接订阅,可以针对不同的消息体做不同的处理
        /// </summary>
        /// <param name="Queue"></param>
        private void SubscribeBymuiMessage(string Queue)
        {
            CheckConnected();
            MyMessage<string> imymes;
            MyOtherMessage deliveredMyOtherMessage;
            bus.Receive(Queue, x => x.Add<MyMessage<string>>(message => imymes = message).Add<MyOtherMessage>(message => deliveredMyOtherMessage = message));
            //您可以设置多个接收器对不同消息类型在同一队列使用接收过载,需要一个行动
        }
        /// <summary>
        /// 只有队列的直接订阅,可以针对不同的消息体做不同的处理,同时监控客户信息
        /// </summary>
        /// <param name="Queue"></param>
        private void SubscribeByContentAndConstomer(string Queue)
        {
            CheckConnected();
            MyMessage<string> imymes;
            MyOtherMessage deliveredMyOtherMessage;
            //您可以设置多个接收器对不同消息类型在同一队列使用接收过载,需要一个行动
            bus.Receive(Queue, x => x.Add<MyMessage<string>>(message =>
            {
                imymes = message;
                Console.WriteLine("TestMessagesQueue1: '{0}'", message.MessageBody);

            })
            .Add<MyOtherMessage>(message => deliveredMyOtherMessage = message), y =>
            {
                Console.WriteLine("TestMessagesQueue1: '{0}'", y.PrefetchCount);

            });

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
