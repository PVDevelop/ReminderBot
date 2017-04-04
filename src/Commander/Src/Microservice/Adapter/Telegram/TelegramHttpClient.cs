using System;
using System.Threading;
using PVDevelop.ReminderBot.Microservice.Port.Http;
using PVDevelop.ReminderBot.Microservice.Port.Telegram;
using PVDevelop.ReminderBot.Microservice.Request;
using PVDevelop.ReminderBot.Microservice.Response;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Telegram
{
    public class TelegramHttpClient : ITelegramClient
    {
        private readonly IHttpClient _httpClient;
        private readonly TelegramApiSettings _telegramApiSettings;

        public TelegramHttpClient(
            IHttpClient httpClient,
            TelegramApiSettings telegramApiSettings)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            if (telegramApiSettings == null) throw new ArgumentNullException(nameof(telegramApiSettings));

            _httpClient = httpClient;
            _telegramApiSettings = telegramApiSettings;
        }

        public ResponseDto<UpdateDto[]> GetUpdates(GetUpdatesDto getUpdatesDto, CancellationToken token)
        {
            if (getUpdatesDto == null) throw new ArgumentNullException(nameof(getUpdatesDto));

            var uri = new Uri($"{_telegramApiSettings.HostAddress}/bot{_telegramApiSettings.Token}/getUpdates");

            return _httpClient.PostJson<ResponseDto<UpdateDto[]>>(uri, getUpdatesDto, token);
        }
    }
}
