using System.Threading;
using System.Threading.Tasks;

namespace PVDevelop.ReminderBot.Microservice.Port.Infrastructure
{
    public interface IRunable
    {
        Task Run(CancellationToken token);
    }
}
