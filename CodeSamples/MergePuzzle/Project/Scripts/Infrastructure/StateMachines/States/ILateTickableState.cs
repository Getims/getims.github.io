namespace Project.Scripts.Infrastructure.StateMachines.States
{
    public interface ILateTickableState
    {
        void LateTick();
    }
}