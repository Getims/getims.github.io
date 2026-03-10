using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Infrastructure.Events;

namespace Project.Scripts.Runtime.Events
{
    public class GameEventsProvider : GameEventProvider
    {
        public PlayerLandedEvent PlayerLandedNewPlatformEvent { get; } = new PlayerLandedEvent();
        public PlayerLandedEvent PlayerLandedSamePlatformEvent { get; } = new PlayerLandedEvent();
        public PlatformCreateEvent PlatformCreateEvent { get; } = new PlatformCreateEvent();
        public PlayerDeadEvent PlayerDeadEvent { get; } = new PlayerDeadEvent();
        public PlayerJumpEvent PlayerJumpEvent { get; } = new PlayerJumpEvent();
    }

    public class PlayerLandedEvent : GameEvent<IPlatform>
    {
    }

    public class PlatformCreateEvent : GameEvent<IPlatform>
    {
    }

    public class PlayerDeadEvent : GameEvent
    {
    }

    public class PlayerJumpEvent : GameEvent
    {
    }
}