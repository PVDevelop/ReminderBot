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
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<ILogger, Logger>()
                .AddTransient<IService, Service>()
                .AddTransient<IDeliveryMessage, DeliveryMessage>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger>();
            var service = serviceProvider.GetService<IService>();

            logger.LogMessage("Start up", null);

            service.ActionFromQueue(default(CancellationToken));

            logger.LogMessage("Exit", null);
        }
    }
}
