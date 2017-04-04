using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using PVDevelop.ReminderBot.Microservice.Adapter.Bus;
using PVDevelop.ReminderBot.Microservice.Adapter.Http;
using PVDevelop.ReminderBot.Microservice.Adapter.Infrastructure;
using PVDevelop.ReminderBot.Microservice.Adapter.Persistance;
using PVDevelop.ReminderBot.Microservice.Adapter.Telegram;
using PVDevelop.ReminderBot.Microservice.Application;
using PVDevelop.ReminderBot.Microservice.Logging;
using PVDevelop.ReminderBot.Microservice.Port.Bus;
using PVDevelop.ReminderBot.Microservice.Port.Http;
using PVDevelop.ReminderBot.Microservice.Port.Infrastructure;
using PVDevelop.ReminderBot.Microservice.Port.Persistance;
using PVDevelop.ReminderBot.Microservice.Port.Telegram;
using StructureMap;

namespace PVDevelop.ReminderBot.Microservice
{
    public class ApplicationBuilder
    {
        private static readonly ILogger Logger = LoggerHelper.GetLogger<ApplicationBuilder>();

        private readonly List<string> _settings = new List<string>();

        private readonly string _configurationPath;

        public ApplicationBuilder(
            string configurationPath)
        {
            if (configurationPath == null) throw new ArgumentNullException(nameof(configurationPath));

            _configurationPath = configurationPath;

            Logger.Info($"Configuration path: {_configurationPath}.");
        }

        public ApplicationBuilder WithJsonSettings(string jsonSettingsFileName)
        {
            if (jsonSettingsFileName == null) throw new ArgumentNullException(nameof(jsonSettingsFileName));

            _settings.Add(jsonSettingsFileName);

            return this;
        }

        public IRunable Build()
        {
            var configurationRoot = BuildConfigurationRoot();

            var dataDirectory = BuildDataDirectory(configurationRoot);

            var container = BuildContainer(configurationRoot, dataDirectory);

            try
            {
                var runables = container.GetAllInstances<IRunable>().ToArray();
                var initializables = container.GetAllInstances<IInitializable>().ToArray();

                return new Adapter.Infrastructure.Application(new[] { container }, runables, initializables);
            }
            catch
            {
                container.Dispose();
                throw;
            }
        }

        private string BuildDataDirectory(IConfigurationRoot configurationRoot)
        {
            var dataDirectory = configurationRoot.GetConnectionString("DataDirectory");

            if (string.IsNullOrWhiteSpace(dataDirectory))
            {
                throw new InvalidOperationException("Data directory not set. Specify 'DataDirectory' variable using settings.");
            }

            Logger.Info($"Data directory: {dataDirectory}.");

            if (!Directory.Exists(dataDirectory))
            {
                Logger.Info($"Creating directory {dataDirectory}.");

                Directory.CreateDirectory(dataDirectory);
            }

            return dataDirectory;
        }

        private IConfigurationRoot BuildConfigurationRoot()
        {
            var configurationBuilder =
                new ConfigurationBuilder().
                    SetBasePath(_configurationPath);

            foreach (var jsonSettings in _settings)
            {
                configurationBuilder.AddJsonFile(jsonSettings, optional: false);
            }

            return configurationBuilder.Build();
        }

        private IContainer BuildContainer(
            IConfigurationRoot configurationRoot,
            string dataDirectory)
        {
            var container = new Container();

            try
            {
                container.Configure(x => ConfigureContainer(configurationRoot, dataDirectory, x));
                container.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, $"Error while configuring application. Container state: {container.WhatDoIHave()}.");
                container.Dispose();
                throw;
            }

            return container;
        }

        private void ConfigureContainer(
            IConfigurationRoot configurationRoot,
            string dataDirectory,
            ConfigurationExpression expression)
        {
            UseTelegramApiSettings(configurationRoot, expression);

            expression.For<IRunable>().Use<UpdatesPuller>().Singleton();
            expression.For<IHttpClient>().Use<HttpClient>().Singleton();
            expression.For<ITelegramClient>().Use<TelegramHttpClient>().Singleton();

            InjectUpdatePositionRepository(dataDirectory, expression);

            InjectMessageBusses(configurationRoot, expression);

            foreach (var commandHandler in GetCommandHandlers())
            {
                expression.For(commandHandler);
            }
        }

        private void InjectUpdatePositionRepository(
            string dataDirectory,
            ConfigurationExpression expression)
        {
            expression.
                For<IUpdatesPositionRepository>().
                Use<FileUpdatesPositionRepository>().
                Ctor<string>().
                Is(dataDirectory).
                Singleton();
        }

        private void InjectMessageBusses(IConfigurationRoot configurationRoot, ConfigurationExpression expression)
        {
            // ВАЖНО! Не использовать DecorateAllWith для IMessageBus. 
            // По непонятной причине тест зависает после исполнения TearDown,
            // когда больше одного декоратора!
            expression.
                For<IMessageBus>().
                Use<InMemoryMessageBus>().
                Named(nameof(InMemoryMessageBus)).
                Ctor<IEnumerable>().
                Is(ctx => GetCommandHandlers().Select(ctx.GetInstance)).
                Singleton();

            expression.
                For<IMessageBus>().
                Use<MessageBusLogger>().
                Ctor<IMessageBus>().
                IsNamedInstance(nameof(RmqMessageBus)).
                Singleton();

            InjectRmqMessageBus(configurationRoot, expression);
        }

        private static void InjectRmqMessageBus(
            IConfigurationRoot configurationRoot,
            ConfigurationExpression expression)
        {

            expression.
                For<IMessageBus>().
                Use<RmqMessageBus>().
                Named(nameof(RmqMessageBus)).
                Ctor<string>().
                Is(() => GetRmqConnectionString(configurationRoot)).
                Ctor<IMessageBus>().
                IsNamedInstance(nameof(InMemoryMessageBus)).
                Singleton();

            expression.Forward<IMessageBus, IInitializable>();
        }

        private static string GetRmqConnectionString(IConfigurationRoot configurationRoot)
        {
            var connectionString = configurationRoot.GetConnectionString("RabbitMq");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("RabbitMq connection string is not set.");
            }

            return connectionString;
        }

        private IEnumerable<Type> GetCommandHandlers()
        {
            yield return typeof(ProcessMessageUpdatesCommandHandler);
            yield return typeof(ExecuteBotCommandHandler);
            yield return typeof(UnknownBotCommandHandler);
        }

        private void UseTelegramApiSettings(
            IConfigurationRoot configurationRoot,
            ConfigurationExpression expression)
        {
            var telegramApiSettingsName = typeof(TelegramApiSettings).Name;
            var telegramSettings = configurationRoot.GetConfiguration<TelegramApiSettings>(telegramApiSettingsName);

            expression.For<TelegramApiSettings>().Use(telegramSettings).Singleton();
        }
    }
}
