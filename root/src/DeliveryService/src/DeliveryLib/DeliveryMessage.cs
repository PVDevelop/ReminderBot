using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace DeliveryLib
{
    public class DeliveryMessage :
        IDeliveryMessage
    {
        private readonly string _apiUrl = null;
        private readonly string _token = null;
        private readonly TimeSpan _pollingTimeout = TimeSpan.FromMinutes(1);

        public DeliveryMessage(
            string apiUrl,
            string token)
        {
            _apiUrl = apiUrl;
            _token = token;
        }

        public async Task SendMessageAsync(string chatId, string message, CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "chat_id", chatId },
                { "text", message }
            };

            var uri = new Uri($"{_apiUrl}{_token}/sendMessage");
            var httpClientHandler = new HttpClientHandler();
            using (var client = new HttpClient(httpClientHandler))
            {
                var payload = JsonConvert.SerializeObject(parameters);
                var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await client
                    .PostAsync(uri, httpContent, cancellationToken)
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
