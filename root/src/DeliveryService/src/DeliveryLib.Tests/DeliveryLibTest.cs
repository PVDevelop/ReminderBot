using System;
using System.Text;
using System.Threading;
using NUnit.Framework;
using DeliveryLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.DotNet.InternalAbstractions;
using RabbitMQ.Client;

namespace DeliveryLib.Tests
{
    [TestFixture]
    public class DeliveryLibTest
    {
        private DeliveryMessage _service = null;

        [SetUp]
        public void Init()
        {
            _service = new DeliveryMessage(
                "https://api.telegram.org/bot",
                "325834582:AAGbvJQ7oYH2E3HW4z_yqshRvu-jT53GOQc");
        }

        [Test]
        public void TestSpamMessage()
        {
            _service.SendMessageAsync("148460428", "test spam message", default(CancellationToken)).Wait();
        }

        [Test]
        public void TestSpamMessageThrowException()
        {
            Assert.Throws<AggregateException>(() => _service.SendMessageAsync("", "test spam message", default(CancellationToken)).Wait());
        }

        [Test]
        public void Service_Queue_Message()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                var message = "{ \"chat_id\": 123, \"text\":Неизвестная команда}";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "pv.telegram.notification",
                                     routingKey: "pv.telegram.notification",
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}
