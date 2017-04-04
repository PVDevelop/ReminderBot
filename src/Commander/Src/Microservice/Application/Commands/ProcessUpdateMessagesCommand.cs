using System;
using System.Collections.Generic;
using PVDevelop.ReminderBot.Microservice.Response;

namespace PVDevelop.ReminderBot.Microservice.Application.Commands
{
    public class ProcessUpdateMessagesCommand
    {
        public IReadOnlyCollection<UpdateDto> Updates { get; }

        public ProcessUpdateMessagesCommand(IReadOnlyCollection<UpdateDto> updates)
        {
            if (updates == null) throw new ArgumentNullException(nameof(updates));

            Updates = updates;
        }
    }
}
