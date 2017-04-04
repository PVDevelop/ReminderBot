using System;

namespace PVDevelop.ReminderBot.Microservice.Port.Bus
{
    public class Message
    {
        public HeadersCollection Headers { get; }

        public object Body { get; }

        public Message(object body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            Headers = new HeadersCollection();

            Body = body;
        }
    }
}
