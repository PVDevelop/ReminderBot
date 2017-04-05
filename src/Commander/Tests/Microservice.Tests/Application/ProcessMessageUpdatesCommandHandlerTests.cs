using System.Collections.Generic;
using PVDevelop.ReminderBot.Microservice.Application;
using PVDevelop.ReminderBot.Microservice.Application.Commands;
using PVDevelop.ReminderBot.Microservice.Response;
using Xunit;

namespace PVDevelop.ReminderBot.Microservice.Tests.Application
{
    public class ProcessMessageUpdatesCommandHandlerTests
    {
        [Fact]
        public void Handle_NullEntities_DoesNotThrowEsception()
        {
            var handler = new ProcessMessageUpdatesCommandHandler(new MessageBusStub());

            var command = new ProcessUpdateMessagesCommand(GetUpdatesWithNullEntities());

            // если не упали, значит тест пройден
            handler.Handle(command);
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

        [Fact]
        public void Handle_NullMessage_DoesNotThrowEsception()
        {
            var handler = new ProcessMessageUpdatesCommandHandler(new MessageBusStub());

            var updateDto = new UpdateDto
            {
                Message = null,
                UpdateId = 3
            };

            var command = new ProcessUpdateMessagesCommand(new[] { updateDto });

            // если не упали, знаит тест пройден
            handler.Handle(command);
        }
    }
}
