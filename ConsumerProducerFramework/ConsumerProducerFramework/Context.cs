using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsumerProducerFramework
{
    public enum RunningStatus : int
    {
        NotStarted = 0,
        Initialized = 1,
        Started = 2,
        Successful = 4,
        Failed = 8
    }
    public class Context
    {
        private List<string> _resultMessages = new List<string>();
        private List<RunningStatus> _result = new List<RunningStatus>();
        public Dictionary<string, object> ExtraData { get; set; }

        public List<string> ResultMessages { get { return _resultMessages; } }
        public List<RunningStatus> Result { get { return _result; } }

        public Context()
        {
            // Nothing to do.
        }

        public void InsertRunningResult(RunningStatus runningStatus, string msg)
        {
            lock (_result)
            {
                _result.Add(runningStatus);
                _resultMessages.Add(msg);
            }
        }
        public string GetRunningResult()
        {
            string msg = string.Empty;
            for (int i = 0; i < _result.Count; i++)
            {
                if (_result[i] != RunningStatus.Successful)
                {
                    msg += _resultMessages[i];
                }
            }

            return msg;
        }
    }
}
