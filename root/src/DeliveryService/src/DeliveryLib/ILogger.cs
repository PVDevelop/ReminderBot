using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryLib
{
    public interface ILogger
    {
        void LogTrace(string message, Exception ex);
        void LogMessage(string message, Exception ex);
        void LogError(string message, Exception ex);
    }
}
