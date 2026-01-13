using Project.Scripts.Configs.Combo;
using Project.Scripts.Infrastructure.Events;

namespace Project.Scripts.Core.Events
{
    public class GameplayEventProvider : GameEventProvider
    {
        public LivesChangedEvent LivesChangedEvent { get; } = new LivesChangedEvent();
        public MoneyChangedEvent MoneyChangedEvent { get; } = new MoneyChangedEvent();
        public LevelSwitchEvent LevelSwitchEvent { get; } = new LevelSwitchEvent();
        public ComboTriggeredEvent ComboTriggeredEvent { get; } = new ComboTriggeredEvent();
        public HintsChangedEvent HintsChangedEvent { get; } = new HintsChangedEvent();
        public HintActivateRequest HintActivateRequest { get; } = new HintActivateRequest();
    }

    public class MoneyChangedEvent : GameEvent<long>
    {
    }

    public class LivesChangedEvent : GameEvent<int>
    {
    }

    public class HintsChangedEvent : GameEvent<int>
    {
    }

    public class HintActivateRequest : GameEvent
    {
    }

    public class LevelSwitchEvent : GameEvent<int>
    {
    }

    public class ComboTriggeredEvent : GameEvent<ComboConfig>
    {
    }
}