using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeliveryLib;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace DeliveryLib
{
    public class Service : 
        IService,
        IDisposable
    {
        private readonly ILogger<Service> _logger;
        private readonly IDeliveryMessage _delivery;
        private IConnection _connection;
        private IModel _channel;

        private readonly string
            _connectionString = null,
            _exchange = null,
            _routingKey = null;
            

        public Service(
            string connectionString,
            string exchange,
            string routingKey,
            ILogger<Service> logger,
            IDeliveryMessage delivery)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (delivery == null)
                throw new ArgumentNullException(nameof(delivery));

            _logger = logger;
            _connectionString = connectionString;
            _exchange = exchange;
            _routingKey = routingKey;
            _delivery = delivery;
        }

        void IService.Run(CancellationToken token)
        {
            var factory = new ConnectionFactory
            {
                Uri = _connectionString,
                AutomaticRecoveryEnabled = true
            };

            _logger.LogMessage($"RabbitMq connection");

            _connection = Policy.
                Handle<BrokerUnreachableException>().
                WaitAndRetryForever((it) =>
                {
                    _logger.LogMessage($"Retry connection ...{it}", null);

                    return TimeSpan.FromSeconds(Math.Min(10, it * 2));
                }).
                Execute(factory.CreateConnection);

            _logger.LogMessage($"RabbitMq connection success");

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchange, ExchangeType.Direct, durable:true);

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName,
                               exchange: _exchange,
                               routingKey: _routingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) => 
            {
                try
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogMessage($"получил сообщение {message}", null);

                    var obj = (dynamic)JsonConvert.DeserializeObject(message);

                    await _delivery.SendMessageAsync(obj["chat_id"].ToString(), obj["text"].ToString(), token);

                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch(Exception ex)
                {
                    _logger.LogError("Recived messaged exception", ex);
                }
            };

            _channel.BasicConsume(queue: queueName,
                                noAck: false,
                                consumer: consumer);

            _logger.LogMessage($"Waiting message from queue");

            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(1000);
            }

        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            _connection = null;
            _channel = null;
        }
    }
}
