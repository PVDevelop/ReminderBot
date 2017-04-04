using System;
using System.Collections;
using System.Collections.Generic;
using PVDevelop.ReminderBot.Microservice.Application.Commands;
using PVDevelop.ReminderBot.Microservice.Port.Bus;
using PVDevelop.ReminderBot.Microservice.Response;

namespace PVDevelop.ReminderBot.Microservice.Application
{
    public class ProcessMessageUpdatesCommandHandler : ICommandHandler<ProcessUpdateMessagesCommand>
    {
        private readonly IMessageBus _messageBus;

        public ProcessMessageUpdatesCommandHandler(IMessageBus messageBus)
        {
            if (messageBus == null) throw new ArgumentNullException(nameof(messageBus));

            _messageBus = messageBus;
        }

        public void Handle(ProcessUpdateMessagesCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            foreach (var nextCommand in GetNextCommands(command.Updates))
            {
                var message = new Message(nextCommand);
                _messageBus.SendMessage(message);
            }
        }

        private IEnumerable GetNextCommands(IReadOnlyCollection<UpdateDto> updates)
        {
            foreach (var updateDto in updates)
                foreach (var command in MapUpdateDtoToCommand(updateDto))
                {
                    yield return command;
                }
        }

        private static IEnumerable MapUpdateDtoToCommand(UpdateDto updateDto)
        {
            var entites = updateDto.Message?.Entities;
            if (entites == null)
            {
                yield break;
            }

            foreach (var messageEntityDto in entites)
            {
                if (messageEntityDto.Type == "bot_command")
                {
                    yield return MapToExecuteBotCommand(updateDto.Message, messageEntityDto);
                }
                else
                {
                    throw new IndexOutOfRangeException($"Unknown message type {messageEntityDto.Type}");
                }
            }
        }

        private static ExecuteBotCommand MapToExecuteBotCommand(
            MessageDto message,
            MessageEntityDto messageEntityDto)
        {
            var botCommand = GetBotCommand(message.Text, messageEntityDto);

            return new ExecuteBotCommand(message.Chat.Id, botCommand);
        }

        private static string GetBotCommand(string messageText, MessageEntityDto messageEntityDto)
        {
            return messageText.Substring(messageEntityDto.Offset + 1, messageEntityDto.Length - 1);
        }
    }
}
