using System;

namespace PVDevelop.ReminderBot.Microservice.Exceptions
{
    public class MicroserviceException : Exception
    {
        public MicroserviceException(string message) :
            base(message)
        {
        }

        public MicroserviceException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
