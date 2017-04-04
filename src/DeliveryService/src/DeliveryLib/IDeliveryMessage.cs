using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeliveryLib
{
    public interface IDeliveryMessage
    {
        Task SendMessageAsync(string charId, string message, CancellationToken token);
    }
}
