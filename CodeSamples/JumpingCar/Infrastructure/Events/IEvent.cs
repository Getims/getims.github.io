namespace Project.Scripts.Infrastructure.Events
{
    public interface IEvent
    {
        int ListenersCount();
    }
}