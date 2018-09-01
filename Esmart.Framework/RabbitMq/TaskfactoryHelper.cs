using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;


namespace Esmart.Framework.RabbitMq
{
//http://developer.51cto.com/art/201407/445556_all.htm
//http://www.cnblogs.com/icyJ/p/Parallel_TaskFactory.html
//http://www.cnblogs.com/TianFang/archive/2012/12/24/2831341.html  asyn task<t> 等待返回值  asyn task  可以等待  asyn void 不等待，不返回
    public class TaskfactoryHelper
    {
        public int maxLength { get; set; }
        public int maxChannel { get; set; }
        public bool isCancel { get; set; }
        Barrier bar;
        private string ProcessList = "* 已完成 0%";
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        Dictionary<string, bool> diclist = new Dictionary<string,bool>(10000);
       // private CancellationTokenSource cts { get; set; }
      CancellationTokenSource cts = new CancellationTokenSource();
        public TaskfactoryHelper(int maxLength, int maxChannel, bool isCancel)
        {
            this.maxLength = maxLength;
            this.maxChannel = maxChannel;
            this.isCancel = isCancel;
            //this.cts = new CancellationTokenSource();
        }
        public static int Test<T1, T2>(Func<T1, T2, int> func, T1 a, T2 b)
        {
            return func(a, b);
        }
        public static void Test1<T>(Action<T> action, T p)
        {
            action(p);
        }

        public virtual void BarrierDoTask<T>(int taskcount, Action<T> OnCancel)// where T : class
        {
            watch.Restart();

            var channels = (taskcount / maxLength) + ((taskcount % maxLength > 0) ? 1 : 0);//总共多少条通道

            var times = (channels / maxChannel) + ((channels % maxChannel > 0) ? 1 : 0);//单服务器分多次

            for (int j = 0; j < times; j++)
            {
                if (isCancel)
                {
                    //OnCancel<T>();
                    break;
                }
                var currChannel = Math.Min(maxChannel, (channels - j * maxChannel));//两者取其小的
                bar = new Barrier(currChannel);//根据次数设置栅栏
                Func<List<string>, int> doSth = subData =>
                {
                    var res = 0;
                    // Connect2WCF.RunSync(sc => res = sc.UpdateMailState(subData, state));
                    return res;
                };
                var tasks = new Action[currChannel];
                for (int i = 0; i < currChannel; i++)
                {

                    tasks[i] = () =>
                    {
                        if (isCancel) return;
                        // var msg = doSth(subData);

                        bar.SignalAndWait();
                    };
                }
                Parallel.Invoke(tasks);
                ProcessList = "* 已完成 " + ((100 * (j + 1) / times)) + "%";
              
            }
        }
        /// <summary>
        /// 任务管理分批处理，如批量插入数据，如执行多线程处理任务。
        /// </summary>
        /// <param name="taskcount"></param>
        /// <param name="OnCancel"></param>
        /// <param name="DoProcess"></param>
        public virtual void  FactoryDoTask(int taskcount, Action<int> OnCancel, Action<int, List<string>> DoProcess)// where T : class
        {
            for (int k = 0; k < taskcount; k++)
			{
                diclist.Add(k.ToString(), true);
			}
          
            var tmpEmails = diclist.Where(x => x.Value).Select(x => x.Key).ToList();
            watch.Restart();
            var channels = (taskcount / maxLength) + ((taskcount % maxLength > 0) ? 1 : 0);//总共多少条通道
            var times = (channels / maxChannel) + ((channels % maxChannel > 0) ? 1 : 0);//单服务器分多次
            if (cts.IsCancellationRequested) return;
            for (int j = 0; j < times; j++)
            {
                int k = j;
                if (cts.Token.IsCancellationRequested)
                {
                    //Console.WriteLine("被取消"); 
                    OnCancel(taskcount);
                    break;
                }
                var currChannel = Math.Min(maxChannel, (channels - j * maxChannel));//两者取其小的
                TaskFactory taskFactory = new TaskFactory();
                Task[] tasks = new Task[currChannel];
                for (int i = 0; i < currChannel; i++)
                {
                    var subData = tmpEmails.Skip((i + j * maxChannel) * maxLength).Take(maxLength).ToList();
                    tasks[i] = new Task(() =>
                        {

                            Thread.Sleep(1000);
                            DoProcess(taskcount, subData);
                            Thread.Sleep(1000);
                        }
                    );
                }
                taskFactory.ContinueWhenAll(tasks,
                    x => taskFactory.StartNew(() =>
                    {
                        ProcessList = "* 已完成 " + ((100 * (k + 1) / times)) + "%";
                    Console.WriteLine(ProcessList);
                    }), CancellationToken.None);
                Array.ForEach(tasks, x => x.Start());
                if(((100 * (k + 1) / times))==50)
                {
                    // cts.Cancel();
                    //Console.ReadKey();
                }
              
            }
        }
        public virtual void  Docanle()
        {
            cts.Cancel();
        }
    }
    
}
