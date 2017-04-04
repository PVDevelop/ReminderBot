using System;
using PVDevelop.ReminderBot.Microservice.Application.Commands;
using PVDevelop.ReminderBot.Microservice.Port.Bus;

namespace PVDevelop.ReminderBot.Microservice.Application
{
    public class ExecuteBotCommandHandler : ICommandHandler<ExecuteBotCommand>
    {
        private readonly IMessageBus _messageBus;

        public ExecuteBotCommandHandler(IMessageBus messageBus)
        {
            if (messageBus == null) throw new ArgumentNullException(nameof(messageBus));

            _messageBus = messageBus;
        }

        public void Handle(ExecuteBotCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var botCommand = CommandBotFactory.CreateBotCommand(command.ChatId, command.Command);

            var message = new Message(botCommand);

            _messageBus.SendMessage(message);
        }
    }
}
