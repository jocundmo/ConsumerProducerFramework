using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class TestProducer1 : ProducerWorker<Book>
    {
        public int Index { get; private set; }
        public TestProducer1(string name, ConsumerWorkerController<Book> ctrl, Context ctx, CultureInfo culture)
            : base(name, ctrl, ctx, culture)
        {
            //this.Name = name;
        }

        protected override void InitializeWorker(Context o)
        {
            Thread.Sleep(6000);
            base.InitializeWorker(o);
        }

        protected override Book PushProduct()
        {
            Thread.Sleep(100);
            if (Index == 20)
            {
                this.EndProduce();
                return null;
            }
            Book b = new Book(Index + Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(string.Format("Producingggggggg...ThreadName is {0}, Name is {1}", Thread.CurrentThread.ManagedThreadId, b.Name));

            Index++;
            return b;
        }
    }
}
