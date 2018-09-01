using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Reflection;
using Esmart.Framework.Exceptions;



namespace Esmart.Framework.Messagging
{
    public class MessageBus:IMessageBus
    {
       
        private static object _instanceLocker = new object();
        private ConcurrentQueue<IMessage> _messaegPool = new ConcurrentQueue<IMessage>();
        private int POLLING_INTERVAL = 300;        
        private Thread _thread;       

        private MessageBus()
        {             
            _thread = new Thread(Subscribe);
            RunIt();
        }

        public static MessageBus Instance
        {
            get {
                return SingletonProxy<MessageBus>.Create(() => new MessageBus());
            }
        }   

        private void ProcessMessage(IMessage message)
        {
            try
            {
                message.ProcessMe();
            }
            catch(Exception ex)
            {
                
                Esmart.Framework.Logging.LogManager.CreateLog4net().Error("applicationError", ex);//LOG4写文本
            }
        }

        private void Subscribe()
        {
            while (true)
            {
                try
                {
                    if (_messaegPool.Count > 0)
                    {
                        IMessage message;
                        if (_messaegPool.TryDequeue(out message))
                        {
                            ProcessMessage(message);
                          
                           
                        }
                    }
                    else
                    {
                        Thread.Sleep(POLLING_INTERVAL);
                    }
                }
                catch(Exception ex)
                {
                   
                    Esmart.Framework.Logging.LogManager.CreateLog4net().Error("applicationError", ex);//LOG4写文本
                }
            }
        }
         
        public void Pubish(IMessage message)
        {
            _messaegPool.Enqueue(message);
        }


        public void RunIt()
        {
            _thread.Start();
        }


        public void StopIt()
        { 
            throw new NotImplementedException();
        }
    }
}
