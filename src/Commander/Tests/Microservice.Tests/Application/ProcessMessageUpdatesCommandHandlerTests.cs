using System.Collections.Generic;
using NUnit.Framework;
using PVDevelop.ReminderBot.Microservice.Application;
using PVDevelop.ReminderBot.Microservice.Application.Commands;
using PVDevelop.ReminderBot.Microservice.Response;

namespace PVDevelop.ReminderBot.Microservice.Tests.Application
{
    [TestFixture]
    public class ProcessMessageUpdatesCommandHandlerTests
    {
        [Test]
        public void Handle_NullEntities_DoesNotThrowEsception()
        {
            var handler = new ProcessMessageUpdatesCommandHandler(new MessageBusStub());

            var command = new ProcessUpdateMessagesCommand(GetUpdatesWithNullEntities());

            handler.Handle(command);

            Assert.Pass();
        }

        private IReadOnlyCollection<UpdateDto> GetUpdatesWithNullEntities()
        {
            var message = new MessageDto
            {
                Entities = null
            };

            return new[]
            {
                new UpdateDto
                {
                    Message = message
                }
            };
        }

        [Test]
        public void Handle_NullMessage_DoesNotThrowEsception()
        {
            var handler = new ProcessMessageUpdatesCommandHandler(new MessageBusStub());

            var updateDto = new UpdateDto
            {
                Message = null,
                UpdateId = 3
            };

            var command = new ProcessUpdateMessagesCommand(new[] { updateDto });

            handler.Handle(command);

            Assert.Pass();
        }
    }
}
