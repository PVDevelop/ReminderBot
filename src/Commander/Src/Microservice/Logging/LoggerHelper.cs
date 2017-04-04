using System;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace PVDevelop.ReminderBot.Microservice.Logging
{
    public static class LoggerHelper
    {
        public static ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T).Name);
        }

        public static ILogger GetLogger(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return new NLogAdapter(name, LogManager.GetLogger(name));
        }

        public static void UseNLogger(
            string configRelativePath)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.UseNLogger(configRelativePath);
        }

        public static void UseNLogger(
            this ILoggerFactory loggerFactory,
            string configRelativePath)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog(configRelativePath);

            PrintGreeting();
        }

        private static void PrintGreeting()
        {
            const string greeting = @"
      ___           ___           ___           ___           ___     
     /\  \         /\  \         /\  \         /\  \         /\  \    
    /::\  \        \:\  \       /::\  \       /::\  \        \:\  \   
   /:/\ \  \        \:\  \     /:/\:\  \     /:/\:\  \        \:\  \  
  _\:\~\ \  \       /::\  \   /::\~\:\  \   /::\~\:\  \       /::\  \ 
 /\ \:\ \ \__\     /:/\:\__\ /:/\:\ \:\__\ /:/\:\ \:\__\     /:/\:\__\
 \:\ \:\ \/__/    /:/  \/__/ \/__\:\/:/  / \/_|::\/:/  /    /:/  \/__/
  \:\ \:\__\     /:/  /           \::/  /     |:|::/  /    /:/  /     
   \:\/:/  /     \/__/            /:/  /      |:|\/__/     \/__/      
    \::/  /                      /:/  /       |:|  |                  
     \/__/                       \/__/         \|__|";

            GetLogger(typeof(LoggerHelper).Name).Info(greeting);
        }
    }
}
