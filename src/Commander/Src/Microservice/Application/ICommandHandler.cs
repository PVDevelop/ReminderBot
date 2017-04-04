namespace PVDevelop.ReminderBot.Microservice.Application
{
    public interface ICommandHandler<in TCommand>
    {
        void Handle(TCommand command);
    }
}
