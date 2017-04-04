using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryLib
{
    public interface ILogger<T>
    {
        void LogTrace(string message, Exception ex = null);
        void LogMessage(string message, Exception ex = null);
        void LogError(string message, Exception ex = null);
    }
}
