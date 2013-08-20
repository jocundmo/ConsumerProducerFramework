using System;
using System.Collections.Generic;
using System.Text;

namespace ConsumerProducerFramework
{
    public class ConsumerWorkerController<T> : IDisposable where T : class
    {
        TaskQueue<T> queue;
        ConsumerWorker<T>[] workers;

        public TaskQueue<T> Queue { get { return queue; } set { queue = value; } }
        public ConsumerWorker<T>[] Workers { get { return workers; } }

        public ConsumerWorkerController(int workerCount)
        {
            queue = new TaskQueue<T>();
            workers = new ConsumerWorker<T>[workerCount];

            //for (int i = 0; i < workerCount; i++)
            //{
            //    workers[i] = ConsumerWorker<T>.CreateConsumerWorker(i.ToString(), queue);
            //    //workers[i] = new TestConsumer1<T>(i.ToString(), queue);
            //}
        }

        public void PushProduct(T prod)
        {
            queue.EnqueueTask(prod);
        }

        public void Dispose()
        {
            // Enqueue one null task per worker to make each exit.
            foreach (ConsumerWorker<T> worker in workers)
                queue.EnqueueTask(default(T));
            foreach (ConsumerWorker<T> worker in workers)
                worker.Join();
            //foreach (Thread worker in workers)
            //    EnqueueTask(null);
            //foreach (Thread worker in workers) 
            //    worker.Join();
        }
    }
}
