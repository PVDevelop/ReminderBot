namespace PVDevelop.ReminderBot.Microservice.Adapter.Persistance
{
    public class UpdatePositionDto
    {
        public long Position { get; }

        public UpdatePositionDto(long position)
        {
            Position = position;
        }
    }
}
