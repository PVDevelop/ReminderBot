using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Polly;
using PVDevelop.ReminderBot.Microservice.Adapter.Infrastructure;
using PVDevelop.ReminderBot.Microservice.Exceptions;
using PVDevelop.ReminderBot.Microservice.Logging;
using PVDevelop.ReminderBot.Microservice.Port.Http;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Http
{
    public class HttpClient : IHttpClient, IDisposable
    {
        private static readonly ILogger Logger = LoggerHelper.GetLogger<HttpClient>();

        private readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

        public TResult PostJson<TResult>(Uri uri, object dto, CancellationToken token)
        {
            try
            {
                return Policy.
                    Handle<AggregateException>(ex => ex.InnerExceptions.Count == 1 && ex.InnerException is HttpRequestException).
                    WaitAndRetryForever(cnt =>
                    {
                        if (cnt % 10 == 0)
                        {
                            Logger.Warning($"Failed to execute http request at {uri}.");
                        }

                        return RetryPolicyHelper.GetWaitPeriod(cnt);
                    }).
                    Execute(() => ExecutePost<TResult>(uri, token, dto));
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1 && ex.InnerException is OperationCanceledException)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        private TResult ExecutePost<TResult>(
            Uri uri,
            CancellationToken token,
            object dto)
        {
            using (var jsonContent = GetJsonContent(dto))
            {
                var httpResponse =
                    _httpClient.
                        PostAsync(uri, jsonContent, token).
                        Result;

                if (!httpResponse.IsSuccessStatusCode)
                {
                    throw new MicroserviceException($"Http respose returned with code {httpResponse.StatusCode}.");
                }

                return GetJsonFromResponse<TResult>(httpResponse);
            }
        }

        private HttpContent GetJsonContent(object content)
        {
            var jsonString = JsonConvert.SerializeObject(content);

            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        private TResult GetJsonFromResponse<TResult>(HttpResponseMessage response)
        {
            var stringContent = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<TResult>(stringContent);

            return result;
        }

        public void Dispose()
        {
            Logger.DecorateDisposingWithLogs(_httpClient.Dispose);
        }
    }
}