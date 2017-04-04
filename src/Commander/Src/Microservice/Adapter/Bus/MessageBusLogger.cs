using System;
using Newtonsoft.Json;
using PVDevelop.ReminderBot.Microservice.Logging;
using PVDevelop.ReminderBot.Microservice.Port.Bus;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Bus
{
    public class MessageBusLogger : IMessageBus
    {
        static readonly ILogger Logger = LoggerHelper.GetLogger<MessageBusLogger>();

        private readonly IMessageBus _next;

        public MessageBusLogger(IMessageBus next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));

            _next = next;
        }

        public void SendMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageString = GetMessageString(message);

            Logger.Debug($"Sending message: {messageString}.");

            _next.SendMessage(message);
        }

        private static string GetMessageString(Message message)
        {
            var bodyType = message.Body.GetType().Name;
            return $"{bodyType}: {JsonConvert.SerializeObject(message)}";
        }
    }
}
