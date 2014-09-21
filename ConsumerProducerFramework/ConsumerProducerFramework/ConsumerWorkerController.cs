using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class ConsumerWorkerController<T> : IDisposable where T : class
    {
        private TaskQueue<T> queue;
        private ConsumerWorker<T>[] consumerWorkers;
        private ProducerWorker<T>[] producerWorkers;
        int consumerWorkerCount = 0;
        int producerWorkerCount = 0;
        public Context Ctx { get; private set; }
        
        private CountdownWaitHandle _initializedConsumersCountDown;
        public CountdownWaitHandle InitializedConsumersCountDown
        {
            get { return _initializedConsumersCountDown; }
        }

        private CountdownWaitHandle _initializedProducersCountDown;
        public CountdownWaitHandle InitializedProducersCountDown
        {
            get { return _initializedProducersCountDown; }
        }

        private CountdownWaitHandle _endedProducerCountDown;
        public CountdownWaitHandle EndedProducerCountDown
        {
            get { return _endedProducerCountDown; }
        }

        public delegate ConsumerWorker<T> CreateConsumerWorkerEvent(ConsumerWorkerController<T> workerController, string consumerName, Context ctx);
        public delegate ProducerWorker<T> CreateProducerWorkerEvent(ConsumerWorkerController<T> workerController, string producerName, Context ctx);
        public event CreateConsumerWorkerEvent CreateConsumerWorker;
        public event CreateProducerWorkerEvent CreateProducerWorker;

        public TaskQueue<T> Queue { get { return queue; } }
        public ConsumerWorker<T>[] ConsumerWorkers { get { return consumerWorkers; } }
        public ProducerWorker<T>[] ProducerWorkers { get { return producerWorkers; } }

        public void Initialize()
        {
            if (consumerWorkerCount <= 0)
                throw new Exception("consumer worker count couldn't less than 0...");
            if (CreateConsumerWorker == null)
                throw new Exception("method to create consumer worker not attached...");
            if (producerWorkerCount <= 0)
                throw new Exception("producer worker count couldn't less than 0...");
            if (CreateProducerWorker == null)
                throw new Exception("method to create producer worker not attached...");

            queue = new TaskQueue<T>();
            consumerWorkers = new ConsumerWorker<T>[consumerWorkerCount];
            producerWorkers = new ProducerWorker<T>[producerWorkerCount];

            _initializedConsumersCountDown = new CountdownWaitHandle(consumerWorkerCount);
            _initializedProducersCountDown = new CountdownWaitHandle(producerWorkerCount);
            _endedProducerCountDown = new CountdownWaitHandle(producerWorkerCount);

            for (int i = 0; i < consumerWorkerCount; i++)
            {
                if (CreateConsumerWorker != null)
                {
                    string consumerName = "c_" + System.IO.Path.GetRandomFileName();
                    Console.WriteLine("About to create sub-thread {0}...", consumerName);
                    consumerWorkers[i] = CreateConsumerWorker(this, consumerName, Ctx);
                }
            }
            for (int i = 0; i < producerWorkerCount; i++)
            {
                if (CreateProducerWorker != null)
                {
                    string producerName = "p_" + System.IO.Path.GetRandomFileName();
                    Console.WriteLine("About to create sub-thread {0}...", producerName);
                    producerWorkers[i] = CreateProducerWorker(this, producerName, Ctx);
                }
            }
        }

        public ConsumerWorkerController(int consumerWorkerCount, int producerWorkerCount, Context ctx)
        {
            this.consumerWorkerCount = consumerWorkerCount;
            this.producerWorkerCount = producerWorkerCount;
            this.Ctx = ctx;
        }

        //public void PushProduct(T prod)
        //{
        //    // Make sure each thread is initialized before sending a prod, 
        //    //!important, otherwise, one thread that is not well initizlied will go into dead loop.
        //    if (!_isCollected)
        //        Collect();
        //    queue.EnqueueTask(prod);
        //}
        private bool _isCollected = false;
        private void Collect()
        {
            Console.WriteLine("Start collect...");
            InitializedConsumersCountDown.Wait();
            InitializedProducersCountDown.Wait();
            _isCollected = true;
            Console.WriteLine("End collect...");
        }

        public void Dispose()
        {
            // Make sure each thread is initialized before sending a prod, 
            //!important, otherwise, one thread that is not well initizlied will go into dead loop.
            if (!_isCollected)
                Collect();
            // Make sure producers are ended.
            EndedProducerCountDown.Wait();
            foreach (ProducerWorker<T> worker in producerWorkers)
            {
                Console.WriteLine("Waiting for producer {0} to join while running status is {1}...", worker.Name, worker.RunningStatus.ToString());
                worker.Join();
            }
            Console.WriteLine("All producers are joined...");
            // Make sure consumers are ended.
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
                foreach (ConsumerWorker<T> worker in consumerWorkers)
                {
                    queue.EnqueueTask(default(T));
                    Console.WriteLine("Sent NULL signal to end sub-thread...");
                }
                foreach (ConsumerWorker<T> worker in consumerWorkers)
                {
                    Console.WriteLine("Waiting for consumer {0} to join while running status is {1}...", worker.Name, worker.RunningStatus.ToString());
                    worker.Join();
                }
            }
            Console.WriteLine("All consumers are joined...");
            HandleException();
            Console.WriteLine("ConsumerWorkController signing off...");
        }

        private void HandleException()
        {
            if (this.Ctx.Result.Count != (consumerWorkerCount + producerWorkerCount))
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
