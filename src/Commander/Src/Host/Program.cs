using System;
using System.Threading;
using System.Threading.Tasks;
using PVDevelop.ReminderBot.Microservice;
using PVDevelop.ReminderBot.Microservice.Logging;

namespace Host
{
    public class Program
    {
        static readonly ILogger Logger;

        static Program()
        {
            LoggerHelper.UseNLogger("nlog.config");

            Logger = LoggerHelper.GetLogger<Program>();
        }

        public static void Main(string[] args)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var applicationTask = RunApplication(cancellationTokenSource.Token);

                Console.CancelKeyPress += (sender, ea) =>
                {
                    try
                    {
                        Logger.Info("Ctrl+C pressed, application will be stopped. Waiting...");

                        cancellationTokenSource.Cancel();
                    }
                    catch (ObjectDisposedException ex)
                    {
                        Logger.Error(ex, "Exception while cancelling token source.");
                    }
                };

                applicationTask.Wait();
            }
        }

        private static Task RunApplication(CancellationToken token)
        {
            return new ApplicationBuilder(AppContext.BaseDirectory).
                    WithJsonSettings("settings.json").
                    Build().
                    Run(token);
        }
    }
}
