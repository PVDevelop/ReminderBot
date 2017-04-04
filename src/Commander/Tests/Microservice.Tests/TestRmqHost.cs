using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Polly;
using PVDevelop.ReminderBot.Microservice.Logging;
using RabbitMQ.Client;

namespace PVDevelop.ReminderBot.Microservice.Tests
{
    public class TestRmqHost : IDisposable
    {
        private static readonly ILogger Logger = LoggerHelper.GetLogger<TestRmqHost>();

        private readonly string _userName;
        private readonly string _password;
        private readonly string _virtualHost;
        private IConnection _connection;
        private IModel _model;

        public TestRmqHost(
            string userName,
            string password,
            string virtualHost)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (virtualHost == null) throw new ArgumentNullException(nameof(virtualHost));

            _userName = userName;
            _password = password;
            _virtualHost = virtualHost;
        }

        public void Start()
        {
            DeleteVirtualHost(TimeSpan.FromSeconds(10));

            CreateVirutalHost();

            SetPermissionsToVirtualHost();

            CreateConnection();
        }

        private void CreateVirutalHost()
        {
            using (var httpClient = CreateHttpClient())
            {
                var vhostPath = GetVirtualHostPath();

                var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                var result = httpClient.PutAsync(vhostPath, httpContent).Result;
                result.EnsureSuccessStatusCode();
            }
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:15672")
            };

            var basicEncodedCredentials = GetBasicEncodedCredentials();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                basicEncodedCredentials);

            return httpClient;
        }

        private string GetBasicEncodedCredentials()
        {
            var basicAuthorizationString = $"{_userName}:{_password}";
            var bytes = Encoding.UTF8.GetBytes(basicAuthorizationString);
            return Convert.ToBase64String(bytes);
        }

        private string GetVirtualHostPath()
        {
            return $"api/vhosts/{_virtualHost}";
        }

        private void SetPermissionsToVirtualHost()
        {
            using (var httpClient = CreateHttpClient())
            {
                var vhostPath = $"/api/permissions/{_virtualHost}/{_userName}";

                const string content = @"{""configure"":"".*"",""write"":"".*"",""read"":"".*""}";
                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

                var result = httpClient.PutAsync(vhostPath, httpContent).Result;
                result.EnsureSuccessStatusCode();
            }
        }

        private void CreateConnection()
        {
            var connectionFactory = new ConnectionFactory
            {
                VirtualHost = _virtualHost,
                UserName = _userName,
                Password = _password,
                Port = 5672,
                HostName = "localhost"
            };

            _connection = connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
        }

        public void Dispose()
        {
            Logger.DecorateDisposingWithLogs(() =>
            {
                try
                {
                    DeleteVirtualHost(TimeSpan.FromSeconds(15));
                }
                catch (Exception ex)
                {
                    Logger.Debug(ex, "Error while deleting virtual host.");
                }

                if (_model != null)
                {
                    _model.Dispose();
                    _model = null;
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            });
        }

        private void DeleteVirtualHost(TimeSpan waitPeriod)
        {
            if (!VHostExists())
            {
                return;
            }
            using (var httpClient = CreateHttpClient())
            {
                var vhostPath = GetVirtualHostPath();
                httpClient.DeleteAsync(vhostPath).Wait(waitPeriod);
            }
        }

        private bool VHostExists()
        {
            using (var httpClient = CreateHttpClient())
            {
                var vHostsResult = httpClient.GetAsync("api/vhosts").Result;
                vHostsResult.EnsureSuccessStatusCode();

                var vHostsStrings = vHostsResult.Content.ReadAsStringAsync().Result;

                var expectedVHostName = string.Format(@"""name"":""{0}""", _virtualHost);

                return vHostsStrings.Contains(expectedVHostName);
            }
        }

        public void DeclareExchange(
            string name,
            string type,
            bool durable)
        {
            _model.ExchangeDeclare(name, type, durable);
        }

        public void DeclareQueue(
            string name)
        {
            _model.QueueDeclare(name);
        }

        public void BindQueue(
            string exchange,
            string queue,
            string routingKey)
        {
            _model.QueueBind(
                queue: queue,
                exchange: exchange,
                routingKey: routingKey);
        }

        public BasicGetResult Get(
            string queue,
            bool ack = false,
            int retryCount = 10,
            int retryPeriodMSec = 1000)
        {
            return Policy<BasicGetResult>.
                HandleResult(result => result == null).
                WaitAndRetry(retryCount, num => TimeSpan.FromMilliseconds(retryPeriodMSec)).
                Execute(() => _model.BasicGet(queue, !ack));
        }
    }
}
