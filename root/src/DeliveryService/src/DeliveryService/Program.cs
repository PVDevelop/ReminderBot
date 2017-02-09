using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using DeliveryLib;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryService
{
    public class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args">
        /// args[0] - apiurl
        /// args[1] - tokenapi
        /// args[2] - hostname
        /// args[3] - exchange
        /// args[4] - routingkey
        /// </param>
        public static void Main(string[] args)
        {
            if (args.Length != 5)
                throw new ArgumentException();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IDeliveryMessage>(it => 
                    new DeliveryMessage(args[0], args[1]))
                .AddSingleton<IService>(it => 
                    new Service(args[2], args[3], args[4], it.GetService<ILogger>(), it.GetService<IDeliveryMessage>()))
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger>();
            var service = serviceProvider.GetService<IService>();

            logger.LogMessage("Start up", null);

            service.ActionFromQueue(default(CancellationToken));

            logger.LogMessage("Exit", null);
        }
    }
}
