using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsumerProducerFramework
{
    public interface IConsumerWorkable<T> where T : class
    {
        bool Async { get; }
        ProducerWorker<T> AsyncProducer { get; }
    }
}
