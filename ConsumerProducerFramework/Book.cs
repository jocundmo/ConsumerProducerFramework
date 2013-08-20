using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsumerProducerFramework
{
    public class Book
    {
        public Book(int i)
        {
            Name = "bookname " + i.ToString();
        }
        public string Name { get; set; }
        public int Age { get; set; }
        //public void Consume()
        //{
        //    Thread.Sleep(2000);
        //    Console.WriteLine(Name + "doing sth...");
        //}
    }
}
