namespace Project.Scripts.Infrastructure.StateMachines
{
    public interface IState
    {
    }

    public interface IEnterState : IState
    {
        void Enter();
    }

    public interface IExitState : IState
    {
        void Exit();
    }

    public interface IEnterState<TParameter> : IState
    {
        void Enter(TParameter payload);
    }

    public interface ITickableState
    {
        void Tick();
    }
}