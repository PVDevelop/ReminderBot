using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PVDevelop.ReminderBot.Microservice.Logging;
using PVDevelop.ReminderBot.Microservice.Port.Infrastructure;

namespace PVDevelop.ReminderBot.Microservice.Tests
{
    public class TestMicroserviceContext : IDisposable
    {
        private static readonly ILogger Logger = LoggerHelper.GetLogger<TestMicroserviceContext>();

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();
        private Task _startTask;
        private IRunable _application;
        private string _dataDirectory;

        public void Initialize(
            string settingsFileName,
            string testSubdirectory)
        {
            if (settingsFileName == null) throw new ArgumentNullException(nameof(settingsFileName));
            if (testSubdirectory == null) throw new ArgumentNullException(nameof(testSubdirectory));

            _dataDirectory = Path.Combine(
                Path.GetTempPath(),
                "PVDevelop",
                "ReminderBot",
                "Commander",
                "Tests",
                testSubdirectory);

            ClearDataDirectory();

            _application =
                new ApplicationBuilder(configurationPath: AppContext.BaseDirectory).
                WithJsonSettings(jsonSettingsFileName: settingsFileName).
                Build();
        }

        public void Start()
        {
            _startTask = _application.Run(_cancellationTokenSource.Token);
        }

        private void ClearDataDirectory()
        {
            if (Directory.Exists(_dataDirectory))
            {
                Directory.Delete(_dataDirectory, true);
            }
        }

        public void Dispose()
        {
            Logger.DecorateDisposingWithLogs(() =>
            {
                _cancellationTokenSource.Cancel();

                if (_startTask != null)
                {
                    try
                    {
                        _startTask.Wait(TimeSpan.FromSeconds(30));
                    }
                    catch (AggregateException ex)
                    {
                        Logger.Info(ex, "Error while waiting for application task.");
                    }
                    _startTask = null;
                }

                _cancellationTokenSource.Dispose();

                ClearDataDirectory();
            });
        }
    }
}
