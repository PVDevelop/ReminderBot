using System;
using System.Threading;
using DeliveryLib;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryService
{
    public class Program
    {
        /// <summary>
        /// </summary>
        public static void Main(string[] args)
        {
            var collection = Environment.GetEnvironmentVariables();

            string apiurl = (string)collection["apiurl"],
                tokenapi = (string)collection["tokenapi"], 
                connectionString = (string)collection["connectionString"], 
                exchange = (string)collection["exchange"], 
                routingkey = (string)collection["routingkey"];
        
            var serviceProvider = new ServiceCollection()
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddSingleton<IDeliveryMessage>(it => 
                    new DeliveryMessage(apiurl, tokenapi))
                .AddSingleton<IService>(it => 
                    new Service(connectionString, exchange, routingkey, it.GetService<ILogger<Service>>(), it.GetService<IDeliveryMessage>()))
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<Program>>();
            var service = serviceProvider.GetService<IService>();

            logger.LogMessage("Starting up application...", null);

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                service.Run(cancellationTokenSource.Token);
                Console.CancelKeyPress += (sender, ea) =>
                {
                    try
                    {
                        logger.LogMessage("Ctrl+C pressed, application will be stopped. Waiting...", null);

                        cancellationTokenSource.Cancel();
                    }
                    catch (ObjectDisposedException ex)
                    {
                        logger.LogError("Exception while cancelling token source.", ex);
                    }
                };
            }

            logger.LogMessage("Application stopped", null);
        }
    }
}
