using System.Linq;
using NUnit.Framework;
using PVDevelop.ReminderBot.Microservice.Application;
using PVDevelop.ReminderBot.Microservice.Application.Commands;
using PVDevelop.ReminderBot.Microservice.Application.MessageHeaders;

namespace PVDevelop.ReminderBot.Microservice.Tests.Application
{
    [TestFixture]
    public class UnknownBotCommandHandlerTests
    {
        [Test]
        public void Handle_SendsMessageWithDestination()
        {
            var messageBusStub = new MessageBusStub();

            var handler = new UnknownBotCommandHandler(messageBusStub);

            var command = new UnknownBotCommand(1, "unknown");

            handler.Handle(command);

            var sentMessage = messageBusStub.SentMessages.Single();

            object headerValue;
            var b = sentMessage.Headers.TryGetHeader(DestinationMessageHeader.HeaderKey, out headerValue);

            Assert.IsTrue(b);
            Assert.IsAssignableFrom(typeof(DestinationMessageHeader), headerValue);
        }
    }
}
