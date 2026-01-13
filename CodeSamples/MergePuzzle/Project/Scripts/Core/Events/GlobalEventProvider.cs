using Project.Scripts.Infrastructure.Events;

namespace Project.Scripts.Core.Events
{
    public class GlobalEventProvider : GameEventProvider
    {
        public TryToSwitchSceneEvent TryToSwitchSceneEvent { get; } = new TryToSwitchSceneEvent();
        public GameLoadCompleteEvent GameLoadCompleteEvent { get; } = new GameLoadCompleteEvent();
        public SoundSwitchEvent SoundSwitchEvent { get; } = new SoundSwitchEvent();
        public MusicSwitchEvent MusicSwitchEvent { get; } = new MusicSwitchEvent();
    }

    public class TryToSwitchSceneEvent : GameEvent<Enums.Scenes>
    {
    }

    public class GameLoadCompleteEvent : GameEvent
    {
    }

    public class SoundSwitchEvent : GameEvent<bool>
    {
    }

    public class MusicSwitchEvent : GameEvent<bool>
    {
    }
}