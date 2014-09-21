using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class TaskQueue<T> where T : class
    {
        object locker = new object();
        Queue<T> taskQ = new Queue<T>();

        public int Count
        {
            get { return taskQ.Count; }
        }

        public TaskQueue()
        {

        }

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
    }
}
