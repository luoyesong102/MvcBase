using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

using System.Reflection;

using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Content;


namespace Esmart.Framework.RabbitMq
{
    public class RabbitMQBus
    {
        private static ConnectionFactory factory;
        private static IConnection connection;
        private static IModel channel;

        private RabbitMQBus()
        {

        }
        public static RabbitMQBus Instance
        {
            get
            {
                return SingletonProxy<RabbitMQBus>.Create(() => new RabbitMQBus());
            }
        }
        private void CheckConnection()
        {
            if (factory == null)
            {
                factory = new ConnectionFactory()
                {
                    HostName = System.Configuration.ConfigurationManager.AppSettings["MessageHostName"],
                    UserName = System.Configuration.ConfigurationManager.AppSettings["MessageUserName"],
                    Password = System.Configuration.ConfigurationManager.AppSettings["MessagePassword"],
                    VirtualHost = System.Configuration.ConfigurationManager.AppSettings["MessageVirtualHost"],
                    RequestedHeartbeat = 0
                 

                };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                //this.Port = 0x1628;
                //this.VirtualHost = "/";
                //this.UserName = "guest";
                //this.Password = "guest";
                //this.RequestedHeartbeat = 10;
                //this.Timeout = 10;
                //this.PublisherConfirms = false;
                //this.PersistentMessages = true;
                //this.CancelOnHaFailover = false;
                //this.UseBackgroundThreads = false;
                //this.PrefetchCount = 50;
                //this.Hosts = new List<HostConfiguration>();
                //this.Ssl = new SslOption();
            }
            
        }
        /// <summary>
        /// 单队列单消费者
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="routingkeyvalue"></param>
        /// <param name="msgModel"></param>
        public  void Pubish<T>(string Change, string routingkeyvalue,  MyMessage<T> msgModel)
        {
            try
            {
                CheckConnection();
          
                var a = Newtonsoft.Json.JsonConvert.SerializeObject(msgModel); ;//这个才是具体的发送内容
                channel.BasicPublish("", "QUEUE_NAME", null, Encoding.UTF8.GetBytes(a));//单一发送
            }
            catch (Exception ex)
            {
                factory = null;
                throw ex;
            }
           
        }
        /// <summary>
        /// 轮询消费
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="routingkeyvalue"></param>
        /// <param name="msgModel"></param>
        public void Pubish1<T>(string Change, string routingkeyvalue, MyMessage<T> msgModel)
        {
            try
            {
                CheckConnection();
                #region  ///构造消息实体对象并发布到消息队列上
                var a = Newtonsoft.Json.JsonConvert.SerializeObject(msgModel); ;//这个才是具体的发送内容

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;//delivery_mode=2指明message为持久的 
               
                 channel.BasicPublish("", "QUEUE_NAME", properties, Encoding.UTF8.GetBytes(a));
            }
            catch (Exception ex)
            {
                factory = null;
                throw ex;
            }

         
                #endregion
        }
        /// <summary>
        /// 公平消费
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="routingkeyvalue"></param>
        /// <param name="msgModel"></param>
        public void Pubish2<T>(string Change, string routingkeyvalue, MyMessage<T> msgModel)
        {
            try
            {
                CheckConnection();
                #region  ///构造消息实体对象并发布到消息队列上
                var a = Newtonsoft.Json.JsonConvert.SerializeObject(msgModel); ;//这个才是具体的发送内容
                var properties = channel.CreateBasicProperties();
                properties.SetPersistent(true);
             
                channel.BasicPublish("", "QUEUE_NAME", properties, Encoding.UTF8.GetBytes(a));
            }
            catch (Exception ex)
            {
                factory = null;
                throw ex;
            }
                #endregion
        }
        /// <summary>
        /// 带交换机的三种路由模式
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="routingkeyvalue"></param>
        /// <param name="msgModel"></param>
        public void Pubish3<T>(string Change, string routingkeyvalue, MyMessage<T> msgModel)
        {
            try
            {
                CheckConnection();
                #region  ///构造消息实体对象并发布到消息队列上
                var a = Newtonsoft.Json.JsonConvert.SerializeObject(msgModel); ;//这个才是具体的发送内容
                var properties = channel.CreateBasicProperties();
                properties.SetPersistent(true);
                channel.BasicPublish(Change, routingkeyvalue, null, Encoding.UTF8.GetBytes(a));
            }
            catch (Exception ex)
            {
                factory = null;
                throw ex;
            }
                #endregion
        }
        /// <summary>
        /// 公平消费
        /// </summary>
        public void SubRecieve()
        {
            CheckConnection();
            var customer = new EventingBasicConsumer(channel);
            channel.BasicQos(0, 1, false);
            customer.Received += ReceiveMessage;
            channel.BasicConsume("queue_name", false, customer);
        }
        /// <summary>
        /// 其他消费
        /// </summary>
        public void SubRecieve1()
        {
            CheckConnection();
            var customer = new EventingBasicConsumer(channel);
            customer.Received += ReceiveMessage;
            channel.BasicConsume("queue_name", false, customer);
        }
        private void ReceiveMessage(object sender, BasicDeliverEventArgs args)
        {

            channel.BasicAck(args.DeliveryTag, false);
            channel.BasicNack(args.DeliveryTag, false, true);
          
        }
        private void QueuebindChange()
        {
            channel.QueueDeclare("queueName", true, false, false, QueueArguments);//QueueArguments就是上面定义的这个dictionarystring queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments

                // string name, 
                // 队列的名称
                //    bool passive = false, 
                //  不要创建队列,如果不存在,false则创建，否则抛出一个异常
                //    bool durable = true, 
                //  可以在服务器重启。如果这是假的队列服务器重启时将被删除。(默认正确)
                //    bool exclusive = false, 
                // 只能由当前连接访问
                //    bool autoDelete = false,
                // 删除队列所有消费者一旦断开连接。(默认错误)
                //    int? perQueueMessageTtl  = null, 
                // 多长时间以毫秒为单位的消息应该保持队列之前就会被丢弃
                //    int? expires = null,
                // 多长时间以毫秒为单位的队列前应该保持未使用自动删除
                //    byte? maxPriority = null,
                // 决定了最大消息优先级队列应该支持
                //    string deadLetterExchange = null, 
                // 决定一个交换的名字可以自动删除前仍未使用的服务器
                //    string deadLetterRoutingKey = null,
                //如果设置,将路由消息的路由关键指定,如果不设置,消息将路由到相同的路由钥匙他们出版。 
                //    int? maxLength = null,
                // 最大数量的就绪队列上的消息可能存在的。消息将被丢弃或死信队列的前面,为新消息一旦达到极限。
                //    int? maxLengthBytes = null队列的最大大小(以字节为单位。消息将被丢弃或死信队列的前面,为新消息一旦达到极限
            channel.ExchangeDeclare("aa", "topic", true, false,null);
            // name:交换你想创造的名称string exchange, string type, bool durable, bool autoDelete
            //type:交换器的类型。它必须是一个有效的AMQP交换类型。使用静态安全属性ExchangeType类的声明交流。
            //passive被动:不要创建一个交换。如果指定的交易不存在,抛出异常。(默认错误)
            //durable:生存服务器重启。如果这个参数是错误的,交易所将被删除当服务器重启。(默认正确)
            //autoDelete:删除这个交易当最后一个队列是不受约束的。(默认错误)
            //@internal:这个交易不能直接使用的出版商,但只有通过交换使用
            //交换绑定。(默认错误)
            //alternateExchange:路由消息交换如果他们不能路由。
            //delayed:如果设置,declars x-delayed-type交换路由延迟信息。


            //ConnectionFactory属性--  channel--属性--交换机属性--队列属性
            //http://www.knowsky.com/898482.html 无交换机的分发机制
            //http://blog.csdn.net/zyz511919766/article/details/41946521 五种场景
            //http://www.cnblogs.com/wangiqngpei557/p/4751124.html 封装技巧
            //http://blog.csdn.net/zyz511919766/article/details/41946521 五种场景
            //http://www.cnblogs.com/aarond/p/rabbitmq.html 队列优先级别
            //http://easynetq.com/  封装DLL(连接生产订阅，监控)--桌面版本操作，web自带
        }
        internal static IDictionary<string, object> QueueArguments
        {
            get
            {
                IDictionary<string, object> arguments = new Dictionary<string, object>();
                arguments["x-max-priority"] = 10;//定义队列优先级为10个级别
                return arguments;
            }
        }
    }

}
