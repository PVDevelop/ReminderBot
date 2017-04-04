using System;

namespace PVDevelop.ReminderBot.Microservice.Application.MessageHeaders
{
    public class DestinationMessageHeader
    {
        public const string HeaderKey = "Destination";

        public string Exchange { get; }

        public string RoutingKey { get; }

        public string ExchangeType { get; }

        public bool Durable { get; }

        public DestinationMessageHeader(
            string exchange,
            string routingKey,
            string exchangeType,
            bool durable)
        {
            if (exchange == null) throw new ArgumentNullException(nameof(exchange));
            if (routingKey == null) throw new ArgumentNullException(nameof(routingKey));
            if (exchangeType == null) throw new ArgumentNullException(nameof(exchangeType));

            Exchange = exchange;
            RoutingKey = routingKey;
            ExchangeType = exchangeType;
            Durable = durable;
        }
    }
}
