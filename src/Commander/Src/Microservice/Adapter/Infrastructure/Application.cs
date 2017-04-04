using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PVDevelop.ReminderBot.Microservice.Logging;
using PVDevelop.ReminderBot.Microservice.Port.Infrastructure;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Infrastructure
{
    public class Application : IRunable
    {
        private static readonly ILogger Logger = LoggerHelper.GetLogger<Application>();

        private readonly IReadOnlyCollection<IDisposable> _disposables;
        private readonly IReadOnlyCollection<IRunable> _runables;
        private readonly IReadOnlyCollection<IInitializable> _initializables;

        public Application(
            IReadOnlyCollection<IDisposable> disposables,
            IReadOnlyCollection<IRunable> runables,
            IReadOnlyCollection<IInitializable> initializables)
        {
            if (disposables == null) throw new ArgumentNullException(nameof(disposables));
            if (runables == null) throw new ArgumentNullException(nameof(runables));
            if (initializables == null) throw new ArgumentNullException(nameof(initializables));

            _disposables = disposables;
            _runables = runables;
            _initializables = initializables;
        }

        public async Task Run(CancellationToken token)
        {
            Logger.Info("Starting application...");

            await Task.Run(() =>
            {
                RunApplication(token);
            });

            Logger.Info("Application has been stopped.");
        }

        private void RunApplication(CancellationToken token)
        {
            Logger.Info("Initializing...");

            Initialize();

            Logger.Info($"{_initializables.Count} initializers have been initialized.");

            Logger.Info("Runnig all runables...");

            var runTasks = RunRunables(token);

            Logger.Info($"{runTasks.Length} tasks have been run.");

            Logger.Info("Application has been started.");

            Logger.Info("Press Ctrl+C to stop application.");

            WaitToken(token);

            Logger.Info("Application is stopping...");

            Logger.Info("Waiting for run tasks to be completed...");

            WaitRunables(runTasks);

            Logger.Info($"{runTasks.Length} tasks have been completed.");

            Logger.Info("Disposing all disposables...");

            DisposeDisposables();

            Logger.Info($"{_disposables.Count} disposables have been disposed.");
        }

        private void Initialize()
        {
            foreach (var initializable in _initializables)
            {
                Logger.Info($"Initializing {initializable}.");

                initializable.Init();

                Logger.Info($"{initializable} has been initialized.");
            }
        }

        private Task[] RunRunables(CancellationToken token)
        {
            return _runables.Select(runable =>
            {
                Logger.Info($"Running {runable}.");

                var task = runable.Run(token);

                Logger.Info($"{runable} has been run.");

                return task;
            }).ToArray();
        }

        private static void WaitToken(CancellationToken token)
        {
            using (var @event = new ManualResetEventSlim())
            {
                try
                {
                    while (true)
                    {
                        @event.Wait(TimeSpan.FromMinutes(1), token);
                        Logger.Info("Ping.");
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private void WaitRunables(Task[] runTasks)
        {
            foreach (var runTask in runTasks)
            {
                Logger.Info("Waiting for task...");

                WaitTask(runTask);

                Logger.Info("Task completed.");
            }
        }

        private void WaitTask(Task task)
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1 && ex.InnerException is OperationCanceledException)
                {
                    return;
                }
                Logger.Error(ex, "Error while waiting for task.");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Unknown error while waiting for task.");
            }
        }

        private void DisposeDisposables()
        {
            foreach (var disposable in _disposables)
            {
                try
                {
                    Logger.Info($"Disposing {disposable}...");

                    disposable.Dispose();

                    Logger.Info($"Disposable {disposable} has been disposed.");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error while disposing object.");
                }
            }
        }
    }
}
