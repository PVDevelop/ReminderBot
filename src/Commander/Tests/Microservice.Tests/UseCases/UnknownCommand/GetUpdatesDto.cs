namespace PVDevelop.ReminderBot.Microservice.Tests.UseCases.UnknownCommand
{
    public class GetUpdatesDto
    {
        public long offset { get; set; }
        public long limit { get; set; }
        public long timeout { get; set; }
        public string[] allowed_updates { get; set; }
    }
}
