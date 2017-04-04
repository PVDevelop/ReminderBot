using System;

namespace PVDevelop.ReminderBot.Microservice.Adapter.Infrastructure
{
    public static class RetryPolicyHelper
    {
        public static TimeSpan GetWaitPeriod(int numOfTry)
        {
            var waitPeriodSec = Math.Min(10, numOfTry * 2);
            return TimeSpan.FromSeconds(waitPeriodSec);
        }
    }
}
