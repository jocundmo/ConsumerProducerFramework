using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class ConsumerWorkerController<T> : IDisposable where T : class
    {
        private TaskQueue<T> queue;
        private ConsumerWorker<T>[] workers;
        int workerCount = 0;
        public Context Ctx { get; private set; }
        private CountdownWaitHandle _initializedThreadsCountDown;
        public CountdownWaitHandle InitializedThreadsCountDown
        {
            get { return _initializedThreadsCountDown; }
        }

        public delegate ConsumerWorker<T> CreateWorkerEvent(ConsumerWorkerController<T> workerController, string workerName, Context ctx);
        public event CreateWorkerEvent CreateWorker;

        public TaskQueue<T> Queue { get { return queue; } }
        public ConsumerWorker<T>[] Workers { get { return workers; } }

        public void Initialize()
        {
            if (workerCount <= 0)
                throw new Exception("worker count couldn't less than 0...");
            if (CreateWorker == null)
                throw new Exception("method to create worker not attached...");
            queue = new TaskQueue<T>();
            workers = new ConsumerWorker<T>[workerCount];
            _initializedThreadsCountDown = new CountdownWaitHandle(workerCount);

            for (int i = 0; i < workerCount; i++)
            {
                if (CreateWorker != null)
                {
                    string randomName = System.IO.Path.GetRandomFileName();
                    Console.WriteLine("About to create sub-thread {0}...", randomName);
                    workers[i] = CreateWorker(this, randomName, Ctx);
                }
            }
        }

        public ConsumerWorkerController(int workerCount, Context ctx)
        {
            this.workerCount = workerCount;
            this.Ctx = ctx;
        }

        public void PushProduct(T prod)
        {
            // Make sure each thread is initialized before sending a prod, 
            //!important, otherwise, one thread that is not well initizlied will go into dead loop.
            if (!_isCollected)
                Collect();
            queue.EnqueueTask(prod);
        }
        private bool _isCollected = false;
        private void Collect()
        {
            Console.WriteLine("Start collect...");
            InitializedThreadsCountDown.Wait();
            _isCollected = true;
            Console.WriteLine("End collect...");
        }

        public void Dispose()
        {
            while (true)
            {
                if (Queue == null)
                {
                    Console.WriteLine("Queue is null...");
                    break;
                }
                else
                {
                    Console.WriteLine("Checking Queue, there is {0} prod in the queue...", Queue.Count);
                    if (Queue.Count <= 0)
                        break;
                    Thread.Sleep(500);
                }
            }
            if (Queue != null)
            {
                // Enqueue one null task per worker to make each exit.
                foreach (ConsumerWorker<T> worker in workers)
                {
                    queue.EnqueueTask(default(T));
                    Console.WriteLine("Sent NULL signal to end sub-thread...");
                }
                foreach (ConsumerWorker<T> worker in workers)
                {
                    Console.WriteLine("Waiting for worker {0} to join while running status is {1}...", worker.Name, worker.RunningStatus.ToString());
                    worker.Join();
                }
            }
            Console.WriteLine("All workers are joined...");
            HandleException();
            Console.WriteLine("ConsumerWorkController signing off...");
        }

        private void HandleException()
        {
            if (this.Ctx.Result.Count != workerCount)
                throw new Exception(string.Format("Sub-threading not completed properly, there are only {0} reported their status...", this.Ctx.Result.Count));

            for (int i = 0; i < this.Ctx.Result.Count; i++)
            {
                if (this.Ctx.Result[i] != RunningStatus.Successful)
                {
                    throw new Exception(string.Format("Sub-thread throw exception... {0}", this.Ctx.ResultMessages[i]));
                }
            }
        }
    }
}
