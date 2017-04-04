using System;
using System.Threading;

namespace PVDevelop.ReminderBot.Microservice.Port.Http
{
    public interface IHttpClient
    {
        TResult PostJson<TResult>(Uri uri, object dto, CancellationToken token);
    }
}
