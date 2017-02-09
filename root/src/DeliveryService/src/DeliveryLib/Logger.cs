using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeliveryLib;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace DeliveryLib
{
    public class Logger : ILogger
    {
        readonly NLog.ILogger _logger = null;

        public Logger()
        {
            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(new ConsoleTarget());
            _logger = LogManager.GetLogger("Logger");
        }

        void ILogger.LogTrace(string message, Exception ex)
        {
            _logger.Trace(ex, message);
        }

        void ILogger.LogMessage(string message, Exception ex)
        {
            _logger.Info(ex, message);
        }

        void ILogger.LogError(string message, Exception ex)
        {
            _logger.Error(ex, message);
        }
    }
}
