using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class TaskQueue<T> where T : class
    {
        object locker = new object();
        //ConsumerWorker<T>[] workers;
        Queue<T> taskQ = new Queue<T>();
        //public event ThreadStart ConsumerStart;


        //public TaskQueue(int workerCount)
        public TaskQueue()
        {
            //Thread.mem
            //workers = new ConsumerWorker<T>[workerCount];

            // Create and start a separate thread for each worker
            //for (int i = 0; i < workerCount; i++)
            //{
            //    workers[i] = new TestConsumer1<T>(i.ToString(), this);
            //    //workers[i].ConsumerStart();
            //    //workers[i].ThreadStart += ConsumerStart;
            //    //workers[i].ThreadStart += new ThreadStart(TaskQueue_ThreadStart);
            //    //(workers[i] = new Thread(Consume)).Start();
            //}
        }

        //public void Dispose()
        //{
        //    // Enqueue one null task per worker to make each exit.
        //    foreach (ConsumerWorker<T> worker in workers)
        //        EnqueueTask(default(T));
        //    foreach (ConsumerWorker<T> worker in workers)
        //        worker.Join();
        //    //foreach (Thread worker in workers)
        //    //    EnqueueTask(null);
        //    //foreach (Thread worker in workers) 
        //    //    worker.Join();
        //}

        public void EnqueueTask(T task)
        {
            lock (locker)
            {
                taskQ.Enqueue(task);
                Monitor.PulseAll(locker);
            }
        }

        public T Dequeue()
        {
            T task;
            lock (locker)
            {
                while (taskQ.Count == 0)
                    Monitor.Wait(locker);
                task = taskQ.Dequeue();
            }
            return task;
            //if (task == null) return null;
        }

        //void Consume()
        //{
        //    while (true)
        //    {
        //        T task;
        //        lock (locker)
        //        {
        //            while (taskQ.Count == 0)
        //                Monitor.Wait(locker);
        //            task = taskQ.Dequeue();
        //            //if (taskQ.Count != 0)
        //            //    task = taskQ.Dequeue();
        //            //else
        //            //    task = default(T);
        //        }
        //        if (task == null) return;         // This signals our exit
        //        //task.Consume();

        //        //Console.Write(task);
        //        //if (task != null)
        //        //    task.Consume();
        //        //Thread.Sleep(1000);              // Simulate time-consuming task
        //    }
        //}
    }
}
