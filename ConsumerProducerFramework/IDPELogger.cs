using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsumerProducerFramework
{
    public interface IDPELogger
    {
        void DebugVerbose(string message, params object[] objs);

        void Debug(string message, params object[] objs);

        void LogInformation(string message, params object[] objs);

        void LogWarning(string message, params object[] objs);

        void LogError(string message, params object[] objs);

        void LogFatal(string message, params object[] objs);

        void Initialize(int logLevel, string logbasePath, string logNamePrefix);

        string GetFullLog();

        string GetLogName();
    }
}
