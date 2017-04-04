using System;
using System.IO;
using Newtonsoft.Json;
using Optional;
using Polly;
using PVDevelop.ReminderBot.Microservice.Adapter.Infrastructure;
using PVDevelop.ReminderBot.Microservice.Application.MessageHeaders;
using PVDevelop.ReminderBot.Microservice.Logging;
using PVDevelop.ReminderBot.Microservice.Port.Bus;
using PVDevelop.ReminderBot.Microservice.Port.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Bus
{
    public class RmqMessageBus :
        IMessageBus,
        IInitializable,
        IDisposable
    {
        private static readonly ILogger Logger = LoggerHelper.GetLogger<RmqMessageBus>();

        private readonly string _connectionString;
        private readonly IMessageBus _next;
        private IConnection _connection;

        public RmqMessageBus(
            string connectionString,
            IMessageBus next)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            if (next == null) throw new ArgumentNullException(nameof(next));

            _connectionString = connectionString;
            _next = next;
        }

        public void SendMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            GetDestination(message).Match(
                some: destination => SendMessageToRmq(destination, message.Body),
                none: () => _next.SendMessage(message));
        }

        private Option<DestinationMessageHeader> GetDestination(Message message)
        {
            object destination;
            if (message.Headers.TryGetHeader(DestinationMessageHeader.HeaderKey, out destination))
            {
                var destinationMessageHeader = (DestinationMessageHeader)destination;
                return destinationMessageHeader.Some();
            }
            return Option.None<DestinationMessageHeader>();
        }

        private void SendMessageToRmq(DestinationMessageHeader destination, object body)
        {
            Logger.Info($"Publishing message to {destination.Exchange}->{destination.RoutingKey}, message: {body}.");

            Policy.
                Handle<AlreadyClosedException>().
                Or<BrokerUnreachableException>().
                WaitAndRetryForever(ProcessRmqPolicyRetry).
                Execute(() => CreateModelAndPublishMessage(destination, body));

            Logger.Info("Message published.");
        }

        private static TimeSpan ProcessRmqPolicyRetry(int cnt)
        {
            if (cnt % 10 == 0)
            {
                Logger.Warning("RabbitMq is unavailable. Retrying...");
            }

            return RetryPolicyHelper.GetWaitPeriod(cnt);
        }

        private void CreateModelAndPublishMessage(DestinationMessageHeader destination, object body)
        {
            using (var model = _connection.CreateModel())
            {
                DeclareExchange(
                    model: model,
                    exchange: destination.Exchange,
                    type: destination.ExchangeType,
                    durable: destination.Durable);

                var bodyBytes = GetBodyBytes(body);
                PublishMessage(model, destination, bodyBytes);
            }
        }

        private static void PublishMessage(IModel model, DestinationMessageHeader destination, byte[] message)
        {
            model.BasicPublish(
                exchange: destination.Exchange,
                routingKey: destination.RoutingKey,
                body: message);
        }

        private void DeclareExchange(
            IModel model,
            string exchange,
            string type,
            bool durable)
        {
            model.ExchangeDeclare(
                exchange: exchange,
                type: type,
                durable: durable);
        }

        private static byte[] GetBodyBytes(object body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                new JsonSerializer().Serialize(streamWriter, body);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        public void Init()
        {
            _connection = CreateConnection();
        }

        private IConnection CreateConnection()
        {
            Logger.Info($"Connecting to message bus at {_connectionString}.");

            var connectionFactory = new ConnectionFactory
            {
                Uri = _connectionString,
                AutomaticRecoveryEnabled = true
            };

            var connection = Policy.
                Handle<BrokerUnreachableException>().
                WaitAndRetryForever(ProcessRmqPolicyRetry).
                Execute(connectionFactory.CreateConnection);

            Logger.Info("Successfully connected to message bus.");

            return connection;
        }

        public void Dispose()
        {
            Logger.DecorateDisposingWithLogs(() =>
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            });
        }
    }
}
