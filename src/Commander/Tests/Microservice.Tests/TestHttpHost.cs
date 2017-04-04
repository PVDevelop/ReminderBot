using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PVDevelop.ReminderBot.Microservice.Logging;

namespace PVDevelop.ReminderBot.Microservice.Tests
{
    public class TestHttpHost : IDisposable
    {
        private static readonly Logging.ILogger Logger = LoggerHelper.GetLogger<TestHttpHost>();

        private readonly int _port;
        private IWebHost _host;

        public TestHttpHost(int port)
        {
            _port = port;
        }

        public void Start()
        {
            _host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseUrls($"http://localhost:{_port}")
                .Build();

            _host.Start();
        }

        public void Dispose()
        {
            Logger.DecorateDisposingWithLogs(() =>
            {
                if (_host != null)
                {
                    _host.Dispose();
                    _host = null;
                }
            });
        }

        private class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc();
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
            {
                app.UseMvc();
            }
        }
    }
}
