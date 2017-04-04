namespace PVDevelop.ReminderBot.Microservice.Port.Persistance
{
    public interface IUpdatesPositionRepository
    {
        long GetPosition();

        void SetPosition(long position);
    }
}
