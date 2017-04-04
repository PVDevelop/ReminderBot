using System;
using PVDevelop.ReminderBot.Microservice.Application.Commands;

namespace PVDevelop.ReminderBot.Microservice.Application
{
    public static class CommandBotFactory
    {
        public static object CreateBotCommand(
            long chatId,
            string command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            switch (command)
            {
                default:
                    return new UnknownBotCommand(chatId, command);
            }
        }
    }
}
