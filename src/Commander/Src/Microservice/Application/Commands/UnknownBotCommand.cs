using System;

namespace PVDevelop.ReminderBot.Microservice.Application.Commands
{
    public class UnknownBotCommand
    {
        public long ChatId { get; }
        public string Command { get; }

        public UnknownBotCommand(
            long chatId,
            string command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            ChatId = chatId;
            Command = command;
        }
    }
}