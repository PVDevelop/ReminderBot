using System.Threading;
using PVDevelop.ReminderBot.Microservice.Request;
using PVDevelop.ReminderBot.Microservice.Response;

namespace PVDevelop.ReminderBot.Microservice.Port.Telegram
{
    public interface ITelegramClient
    {
        ResponseDto<UpdateDto[]> GetUpdates(GetUpdatesDto getUpdatesDto, CancellationToken token);
    }
}
