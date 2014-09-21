﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;

namespace ConsumerProducerFramework
{
    public class ConsumerWorker<T> where T : class
    {
        private object _locker = new object();
        public ConsumerWorkerController<T> Controller { get; private set; }
        public TaskQueue<T> Queue
        {
            get
            {
                return Controller.Queue;
            }
        }
        public string Name
        {
            get
            {
                if (worker != null)
                    return worker.Name;
                else
                    return "";
            }
            set
            {
                if (worker != null)
                    worker.Name = value;
            }
        }
        private Thread worker;
        public Context Ctx;
        public bool CommitLogToDBWhenEnded { get; set; }

        //private RunningStatus _collectingRunningStatus = RunningStatus.NotStarted;
        //public RunningStatus CollectingRunningStatus
        //{
        //    get
        //    {
        //        lock (_locker)
        //        {
        //            return _collectingRunningStatus;
        //        }
        //    }
        //    set
        //    {
        //        lock (_locker)
        //        {
        //            this._collectingRunningStatus = value;
        //        }
        //    }
        //}
        private RunningStatus _runningStatus = RunningStatus.NotStarted;
        public RunningStatus RunningStatus
        {
            get
            {
                lock (_locker)
                {
                    return _runningStatus;
                }
            }
            set
            {
                lock (_locker)
                {
                    this._runningStatus = value;
                }
            }
        }
        public string Message { get; set; }

        public ConsumerWorker(string name, ConsumerWorkerController<T> controller, Context ctx, CultureInfo culture)
        {
            this.Ctx = ctx;
            this.Controller = controller;
            //this.Name = name;
            this.CommitLogToDBWhenEnded = true;

            worker = new Thread(ConsumerStart);
            worker.Name = name;
            worker.CurrentCulture = culture;
            worker.Start(Ctx);

        }

        public void ConsumerStart(object o)
        {
            try
            {
                Context ctx = (Context)o;
                InitializeWorkerInner(ctx);
                while (true)
                {
                    T task = Queue.Dequeue();
                    if (task == null)
                    {

                        Console.WriteLine("Worker {0} received a NULL as end signal...", this.Name);
                        this.Message = "Success";
                        this.RunningStatus = RunningStatus.Successful;
                        this.Ctx.InsertRunningResult(RunningStatus, this.Message);

                        EndWorkerInner();
                        return;
                    }
                    else
                        PullProductInner(task);
                }
            }
            catch (Exception ex)
            {
                this.Message = ex.Message;
                this.RunningStatus = RunningStatus.Failed;
                this.Ctx.InsertRunningResult(RunningStatus, this.Message);
                //this.Ctx.InsertRunningResult(this.IsSuccessful, this.Message);
                Console.WriteLine("Exception found... {0}", ex.ToString());
                EndWorkerInner();
                //throw;
            }
        }

        protected virtual void EndWorker()
        {
            // Nothing to do.
        }
        protected virtual void InitializeWorker(Context o)
        {
            // Nothing to do.
        }
        protected virtual void PreExecuteWorker(Context o)
        {
            // Nothing to do.
        }
        private void EndWorkerInner()
        {
            EndWorker();
            //this.IsSuccessful = true;
            //this.Message = "Success";
            //this.RunningStatus = DataProcessEngine.RunningStatus.Successful;
            //this.Ctx.InsertRunningResult(RunningStatus, this.Message);
            //this.Ctx.InsertRunningResult(this.IsSuccessful, this.Message);
            Console.WriteLine("***********************  Consumer Worker Ended ***********************");
            //if (this.CommitLogToDBWhenEnded)
            //{
            //    try
            //    {
            //        string msg = MessageLoggerManager.GetLogger().GetFullLog();
            //        WriteDBLoggingAction writeDBLoggingAction = Action.CreateAction<WriteDBLoggingAction>("FakeCommonStoryboard",
            //            0, "DataProcessEngine", this.Name, "", "", "DPE - ConsumerWorker", msg, MessageLoggerManager.GetLogger().GetLogName());
            //        writeDBLoggingAction.ByPassValidation = true;
            //        writeDBLoggingAction.Run();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageLoggerManager.GetLogger().LogInformation("*********************** Data Process Engine - Consumer Worker Write DB Logging failed ***********************");
            //        MessageLoggerManager.GetLogger().LogError(ex.ToString());
            //    }
            //    finally
            //    {
            //    }
            //}
        }

        private void InitializeWorkerInner(Context o)
        {
            //int logLevel = 0; // default
            //string logBasePath = @"C:\SSISLogFolder\DevCurrent";// default
            //try
            //{
            //    int.TryParse(EngineConfigurationManager.Instance.GetConfig("root").GetMetadata("LogLevel"), out logLevel);
            //    logBasePath = EngineConfigurationManager.Instance.GetConfig("root").GetMetadata("LogBasePath");
            //}
            //catch { } // ignore any exception here.

            //MessageLoggerManager.Register(logLevel, logBasePath, string.Format("Thread_{0}", Thread.CurrentThread.Name), true, true);
            //MessageLoggerManager.GetLogger().LogInformation("*********************** Data Process Engine - Consumer Worker Started ***********************");
            InitializeWorker(o);
            //this.IsInitialized = true;
            //if (Thread.CurrentThread.ManagedThreadId % 2 == 0)
            //{
            //    MessageLoggerManager.GetLogger().DebugVerbose("Sleep 1000...");
            //    Thread.Sleep(1000);
            //}

            this.RunningStatus = RunningStatus.Initialized;
            Console.WriteLine("Initialized done...ThreadId is {0}, name is {1}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
            Controller.InitializedThreadsCountDown.Signal();
            PreExecuteWorker(o);
            //Thread.Sleep(1000 * 15);
        }



        private void PullProductInner(T task)
        {
            this.RunningStatus = RunningStatus.Started;
            PullProduct(task);
        }
        protected virtual void PullProduct(T task)
        {
            throw new NotImplementedException();
        }

        internal void Join()
        {
            worker.Join();
        }
    }
}
