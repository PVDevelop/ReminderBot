namespace PVDevelop.ReminderBot.Microservice.Port.Bus
{
    public interface IMessageBus
    {
        void SendMessage(Message message);
    }
}
