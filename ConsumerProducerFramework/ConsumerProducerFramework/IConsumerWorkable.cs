using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsumerProducerFramework
{
    public interface IConsumerWorkable<T> where T : class
    {
        bool Async { get; }
        ConsumerWorkerController<T> AsyncController { get; }
    }
}
