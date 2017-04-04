using System;

namespace PVDevelop.ReminderBot.Microservice.Application.Commands
{
    public class ExecuteBotCommand
    {
        public long ChatId { get; }
        public string Command { get; }

        public ExecuteBotCommand(
            long chatId,
            string command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            ChatId = chatId;
            Command = command;
        }
    }
}
