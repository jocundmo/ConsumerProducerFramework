using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;

namespace ConsumerProducerFramework
{
    public class TestConsumer1 : ConsumerWorker<Book>
    {
        //public string Name { get; private set; }

        public TestConsumer1(string name, ConsumerWorkerController<Book> ctrl, Context ctx, CultureInfo culture)
            : base(name, ctrl, ctx, culture)
        {
            //this.Name = name;
        }


        protected override void InitializeWorker(Context o)
        {
            Thread.Sleep(5000);
            base.InitializeWorker(o);
        }
        protected override void PullProduct(Book task)
        {
            //Book b = (Book)task;
            Console.WriteLine(string.Format("Consumed one ... ... ... ThreadName is {0}, Name is {1}", Thread.CurrentThread.ManagedThreadId, task.Name));
            Thread.Sleep(100);
        }
        //public override void ConsumerStart()
        //{
        //    //Console.WriteLine(string.Format("ThreadName is {0}, Name is {1}", Thread.CurrentThread.ManagedThreadId,  Name));

        //    while (true)
        //    {
        //        T task = Queue.Dequeue();
        //        if (task == null)
        //            return;

        //        //lock (locker)
        //        //{
        //        //    while (queue.Count == 0)
        //        //        Monitor.Wait(locker);
        //        //    task = taskQ.Dequeue();
        //        //    //if (taskQ.Count != 0)
        //        //    //    task = taskQ.Dequeue();
        //        //    //else
        //        //    //    task = default(T);
        //        //}
        //        //if (task == null) return;         // This signals our exit
        //        //task.Consume();

        //        //Console.Write(task);
        //        //if (task != null)
        //        //    task.Consume();
        //        //Thread.Sleep(1000);              // Simulate time-consuming task
        //    }
        //}
    }
}
