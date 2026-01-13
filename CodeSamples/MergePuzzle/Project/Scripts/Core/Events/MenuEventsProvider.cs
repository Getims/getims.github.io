using Project.Scripts.Infrastructure.Events;
using UnityEngine;

namespace Project.Scripts.Core.Events
{
    public class MenuEventsProvider
    {
        public MoneySpawnRequest MoneySpawnRequest { get; } = new MoneySpawnRequest();
        public CollectionUnlockedEvent CollectionUnlockedEvent { get; } = new CollectionUnlockedEvent();
    }

    public class MoneySpawnRequest : GameEvent<Vector3>
    {
    }

    public class CollectionUnlockedEvent : GameEvent<int>
    {
    }
}