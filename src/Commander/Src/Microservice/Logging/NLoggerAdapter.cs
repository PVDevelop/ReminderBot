using System;
using NLog;

namespace PVDevelop.ReminderBot.Microservice.Logging
{
    internal class NLogAdapter : ILogger
    {
        private readonly Logger _nLogLogger;

        public string Owner { get; }

        internal NLogAdapter(string owner, Logger nLogLogger)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            if (nLogLogger == null) throw new ArgumentNullException(nameof(nLogLogger));

            Owner = owner;
            _nLogLogger = nLogLogger;
        }

        public void Debug(Exception exception, string message)
        {
            _nLogLogger.Debug(exception, message);
        }

        public void Debug(string message)
        {
            _nLogLogger.Debug(message);
        }

        public void Info(Exception exception, string message)
        {
            _nLogLogger.Info(exception, message);
        }

        public void Info(string message)
        {
            _nLogLogger.Info(message);
        }

        public void Warning(Exception exception, string message)
        {
            _nLogLogger.Warn(exception, message);
        }

        public void Warning(string message)
        {
            _nLogLogger.Warn(message);
        }

        public void Error(Exception exception, string message)
        {
            _nLogLogger.Error(exception, message);
        }

        public void Error(string message)
        {
            _nLogLogger.Error(message);
        }

        public void Fatal(Exception exception, string message)
        {
            _nLogLogger.Fatal(exception, message);
        }

        public void Fatal(string message)
        {
            _nLogLogger.Fatal(message);
        }
    }
}
