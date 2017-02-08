using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeliveryLib;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DeliveryService
{
    public class Service : IService
    {
        private readonly ILogger _logger;
        private readonly IDeliveryMessage _delivery;
        private readonly string _hostname = null, _exchange = null, _routingKey = null;

        public Service(
            string hostname,
            string exchange,
            string routingKey,
            ILogger logger,
            IDeliveryMessage delivery)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (delivery == null)
                throw new ArgumentNullException(nameof(delivery));

            _logger = logger;
            _hostname = hostname;
            _exchange = exchange;
            _routingKey = routingKey;
            _delivery = delivery;
        }

        void IService.ActionFromQueue(CancellationToken token)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(_exchange, ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: _exchange,
                                  routingKey: _routingKey);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);

                        _delivery.SendMessageAsync(message, message, token);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError("При отправке сообщения произошла ошибка", ex);
                    }
                    
                };
                channel.BasicConsume(queue: queueName,
                                     noAck: true,
                                     consumer: consumer);

                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
