using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace DeliveryLib
{
    public class DeliveryMessage :
        IDeliveryMessage,
        IDisposable
    {
        private readonly string _apiUrl = null;
        private readonly string _token = null;
        private readonly TimeSpan _pollingTimeout = TimeSpan.FromMinutes(1);
        private readonly HttpClient _client = null;

        public DeliveryMessage(
            string apiUrl,
            string token)
        {
            if (String.IsNullOrEmpty(apiUrl))
                throw new ArgumentNullException(nameof(apiUrl));

            if (String.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            _apiUrl = apiUrl;
            _token = token;
            _client = new HttpClient();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public async Task SendMessageAsync(string chatId, string message, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "chat_id", chatId },
                { "text", message }
            };

            var uri = new Uri($"{_apiUrl}{_token}/sendMessage");
            var payload = JsonConvert.SerializeObject(parameters);
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _client
                .PostAsync(uri, httpContent, cancellationToken)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
        }
    }
}
