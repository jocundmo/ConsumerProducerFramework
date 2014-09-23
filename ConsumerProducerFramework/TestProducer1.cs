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
        //public int Index { get; private set; }
        public TestProducer1(string name, ConsumerWorkerController<Book> ctrl, Context ctx, CultureInfo culture)
            : base(name, ctrl, ctx, culture)
        {
            //this.Name = name;
        }

        protected override void InitializeWorker(Context o)
        {
            base.InitializeWorker(o);
        }
        //List<Book> blist = new List<Book>();
        protected override void StartProduceProduct(Context o)
        {
            for (int i = 0; i < 10; i++)
            {
                Book b = new Book(i + Thread.CurrentThread.ManagedThreadId);
                this.PushProduct(b);
                Console.WriteLine(string.Format("Producingggggggg...ThreadName is {0}, Name is {1}", Thread.CurrentThread.ManagedThreadId, b.Name));
                //blist.Add(b);
                //Thread.Sleep(2500);
                //b.Consume();
            }
            this.EndProduce();
        }
        //protected override void PrepareProduct()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        Book b = new Book(i);
        //        blist.Add(b);
        //        //Thread.Sleep(2500);
        //        //b.Consume();
        //    }
        //}

        //protected override Book PushProduct()
        //{
        //    Thread.Sleep(100);
        //    if (Index == 20)
        //    {
        //        return this.EndProduce();
        //        //return null;
        //    }
        //    Book b = new Book(Index + Thread.CurrentThread.ManagedThreadId);
        //    Console.WriteLine(string.Format("Producingggggggg...ThreadName is {0}, Name is {1}", Thread.CurrentThread.ManagedThreadId, b.Name));

        //    Index++;
        //    return b;
        //}
    }
}
