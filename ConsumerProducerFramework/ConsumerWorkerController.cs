using System;
using System.Collections.Generic;
using System.Text;

namespace ConsumerProducerFramework
{
    public class ConsumerWorkerController<T> : IDisposable where T : class
    {
        TaskQueue<T> queue;
        ConsumerWorker<T>[] workers;
        int workerCount = 0;

        public delegate ConsumerWorker<T> CreateWorkerEvent(ConsumerWorkerController<T> workerController, string workerName);
        public event CreateWorkerEvent CreateWorker;
        //public event Action CreateWorker;

        public TaskQueue<T> Queue { get { return queue; } set { queue = value; } }
        public ConsumerWorker<T>[] Workers { get { return workers; } }

        public void Initialize()
        {
            if (workerCount <= 0)
                throw new Exception("worker count couldn't less than 0...");
            if (CreateWorker == null)
                throw new Exception("method to create worker not attached...");
            queue = new TaskQueue<T>();
            workers = new ConsumerWorker<T>[workerCount];

            for (int i = 0; i < workerCount; i++)
            {
                if (CreateWorker != null)
                    workers[i] = CreateWorker(this, i.ToString());
            }
        }

        public ConsumerWorkerController(int workerCount)
        {
            this.workerCount = workerCount;
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
