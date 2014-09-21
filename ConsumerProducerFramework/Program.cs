using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace ConsumerProducerFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Context ctx = new Context();
            //using（ConsumerWorkerController<Book> conumsers = new ConsumerWorkerController<Book>(
            using (ConsumerWorkerController<Book> ctrl = new ConsumerWorkerController<Book>(10, ctx))
            {
                //ctrl.InitializeWorkers<Book>();
                ctrl.CreateWorker += new ConsumerWorkerController<Book>.CreateWorkerEvent(ctrl_CreateWorker);
                ctrl.Initialize();
                //for (int i = 0; i < 5; i++)
                //{
                //    ctrl.Workers[i] = new TestConsumer1(i.ToString(), ctrl);
                //}
                //TaskQueue<Book> queue = new TaskQueue<Book>();
                //ctrl.que
                for (int i = 0; i < 10; i++)
                {
                    Book b = new Book(i);
                    Thread.Sleep(250);
                    ctrl.PushProduct(b);
                    //b.Consume();
                }
            }

            //using (TaskQueue<Book> queue = new TaskQueue<Book>(5))
            //{
            //    for (int i = 0; i < 10; i++)
            //    {
            //        Book b = new Book(i);
            //        //Thread.Sleep(2500);
            //        queue.EnqueueTask(b);
            //        //b.Consume();
            //    }
            //}

            sw.Stop();
            Console.WriteLine("Last Seconds: " + sw.Elapsed.Seconds);
            Console.Read();
        }

        static ConsumerWorker<Book> ctrl_CreateWorker(ConsumerWorkerController<Book> workerController, string workerName, Context ctx)
        {
            return new TestConsumer1(workerName, workerController, ctx, System.Globalization.CultureInfo.CurrentCulture);
        }

        
    }
}
