using System;

namespace PVDevelop.ReminderBot.Microservice.Logging
{
    public static class LoggerExtensions
    {
        public static void DecorateDisposingWithLogs(this ILogger logger, Action disposeAction)
        {
            logger.Info("Disposing...");

            try
            {
                disposeAction();

                logger.Info("Object has been disposed.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error while disposing {logger.Owner}.");
            }
        }
    }
}
