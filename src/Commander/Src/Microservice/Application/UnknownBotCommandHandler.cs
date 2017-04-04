using System;
using PVDevelop.ReminderBot.Microservice.Application.Commands;
using PVDevelop.ReminderBot.Microservice.Application.MessageHeaders;
using PVDevelop.ReminderBot.Microservice.Port.Bus;
using RabbitMQ.Client;

namespace PVDevelop.ReminderBot.Microservice.Application
{
    public class UnknownBotCommandHandler : ICommandHandler<UnknownBotCommand>
    {
        private readonly IMessageBus _messageBus;

        public UnknownBotCommandHandler(IMessageBus messageBus)
        {
            if (messageBus == null) throw new ArgumentNullException(nameof(messageBus));

            _messageBus = messageBus;
        }

        public void Handle(UnknownBotCommand command)
        {
            var message = CreateMessage(command);

            _messageBus.SendMessage(message);
        }

        private Message CreateMessage(UnknownBotCommand command)
        {
            var nextCommand = new PrintMessageCommand(
                chatId: command.ChatId,
                text: $"Неизвестная команда {command.Command}.");

            var message = new Message(nextCommand);

            var destination = new DestinationMessageHeader(
                exchange: "pv.telegram.message",
                routingKey: "telegram",
                exchangeType: ExchangeType.Direct,
                durable: true);

            message.Headers.AddHeader(DestinationMessageHeader.HeaderKey, destination);

            return message;
        }
    }
}
