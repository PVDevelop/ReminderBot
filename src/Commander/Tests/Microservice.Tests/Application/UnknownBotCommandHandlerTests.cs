using System.Linq;
using PVDevelop.ReminderBot.Microservice.Application;
using PVDevelop.ReminderBot.Microservice.Application.Commands;
using PVDevelop.ReminderBot.Microservice.Application.MessageHeaders;
using Xunit;

namespace PVDevelop.ReminderBot.Microservice.Tests.Application
{
    public class UnknownBotCommandHandlerTests
    {
        [Fact]
        public void Handle_SendsMessageWithDestination()
        {
            var messageBusStub = new MessageBusStub();

            var handler = new UnknownBotCommandHandler(messageBusStub);

            var command = new UnknownBotCommand(1, "unknown");

            handler.Handle(command);

            var sentMessage = messageBusStub.SentMessages.Single();

            object headerValue;
            var hasHeader = sentMessage.Headers.TryGetHeader(DestinationMessageHeader.HeaderKey, out headerValue);

            Assert.True(hasHeader);
            Assert.IsAssignableFrom(typeof(DestinationMessageHeader), headerValue);
        }
    }
}
