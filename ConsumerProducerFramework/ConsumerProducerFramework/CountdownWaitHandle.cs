using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class CountdownWaitHandle
    {
        object _locker = new object();
        int _value;

        public CountdownWaitHandle() { }
        public CountdownWaitHandle(int initialCount) { _value = initialCount; }

        public void Signal()
        {
            Console.WriteLine("set...");
            AddCount(-1);
        }

        private void AddCount(int amount)
        {
            lock (_locker)
            {
                _value += amount;
                if (_value <= 0) Monitor.PulseAll(_locker);
            }
        }

        public void Wait()
        {
            lock (_locker)
                while (_value > 0)
                    Monitor.Wait(_locker);
        }
    }
}
