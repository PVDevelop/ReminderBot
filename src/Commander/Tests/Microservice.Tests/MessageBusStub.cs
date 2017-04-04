using System.Collections.Generic;
using PVDevelop.ReminderBot.Microservice.Port.Bus;

namespace PVDevelop.ReminderBot.Microservice.Tests
{
    internal class MessageBusStub : IMessageBus
    {
        public List<Message> SentMessages { get; } = new List<Message>();

        public void SendMessage(Message message)
        {
            SentMessages.Add(message);
        }
    }
}
