using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using PVDevelop.ReminderBot.Microservice.Logging;

namespace PVDevelop.ReminderBot.Microservice.Tests.UseCases
{
    [TestFixture]
    public class UnknownCommandTests
    {
        const string ExchangeName = "pv.telegram.message";
        const string ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
        const string QueueName = "test";
        const string RoutingKey = "telegram";

        [Test]
        public void UnknownCommand_SendsUnknownCommandMessageToMessageBus()
        {
            var getResult = _rmqHost.Get(QueueName);

            Assert.NotNull(getResult, "Expected message has not been received.");

            var unknownCommandContent = DeserializeContent(getResult.Body);

            Assert.NotNull(unknownCommandContent);
            Assert.AreEqual(123, unknownCommandContent.chat_id);
            Assert.AreEqual("Неизвестная команда hi.", unknownCommandContent.text);
        }

        private UnknownCommandContent DeserializeContent(byte[] body)
        {
            using (var memoryStream = new MemoryStream(body))
            using (var streamReader = new StreamReader(memoryStream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return new JsonSerializer().Deserialize<UnknownCommandContent>(jsonReader);
            }
        }

        private TestMicroserviceContext _microserviceContext;
        private TestHttpHost _httpHost;
        private TestRmqHost _rmqHost;

        [SetUp]
        public void Setup()
        {
            LoggerHelper.UseNLogger("nlog.config");

            StartRmqHost();

            StartHttpHost();

            StartMicroservice();
        }

        private void StartRmqHost()
        {
            _rmqHost = new TestRmqHost(
                userName: "guest",
                password: "guest",
                virtualHost: "test_unknown_command");

            _rmqHost.Start();

            _rmqHost.DeclareExchange(name: ExchangeName, type: ExchangeType, durable: true);
            _rmqHost.DeclareQueue(name: QueueName);
            _rmqHost.BindQueue(exchange: ExchangeName, queue: QueueName, routingKey: RoutingKey);
        }

        private void StartHttpHost()
        {
            _httpHost = new TestHttpHost(7000);

            _httpHost.Start();
        }

        private void StartMicroservice()
        {
            _microserviceContext = new TestMicroserviceContext();

            _microserviceContext.Initialize(
                settingsFileName: @"UseCases\UnknownCommand\UnknownCommandSettings.json",
                testSubdirectory: "UnknownCommand");

            _microserviceContext.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _microserviceContext?.Dispose();
            _rmqHost?.Dispose();
            _httpHost?.Dispose();
        }

        private class UnknownCommandContent
        {
            public long chat_id { get; set; }

            public string text { get; set; }
        }
    }
}
