using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class ConsumerWorker<T> where T : class
    {
        public TaskQueue<T> Queue { get; private set; }
        public string Name { get; private set; }
        private Thread worker;

        public ConsumerWorker(string name, TaskQueue<T> queue)
        {
            this.Queue = queue;
            this.Name = name;
            worker = new Thread(ConsumerStart);
            worker.Name = name;
            worker.Start();
        }

        public void ConsumerStart()
        {
            while (true)
            {
                T task = Queue.Dequeue();
                if (task == null)
                    return;
                else
                    PullProduct(task);
            }

            //PullProduct(task
            //worker.Start();
            //ConsumerStartWork();
        }

        protected virtual void PullProduct(T task)
        {
            throw new NotImplementedException();
        }
        //protected virtual void ConsumerStartWork();

        internal void Join()
        {
            worker.Join();
        }

        //internal static ConsumerWorker<T> CreateConsumerWorker(string p, TaskQueue<T> queue)
        //{
        //    return new TestConsumer1(p, queue);
        //    //return new TestConsumer1(p, queue
        //}
    }
}
