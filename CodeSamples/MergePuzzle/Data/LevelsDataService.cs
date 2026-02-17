using Project.Scripts.Infrastructure.Data;
using Project.Scripts.Infrastructure.Data.Experimental;
using Project.Scripts.Runtime.Events;

namespace Project.Scripts.Data
{
    public interface ILevelsDataService
    {
        public DataValue<LevelsData, int> CurrentLevel { get; }
        public int CurrentLevelUI { get; }
        public DataValue<LevelsData, int> LastPassedLevel { get; }
        public DataValue<LevelsData, int> CollectionsUnlocked { get; }

        public void ActualizeLevelsData();
    }

    public class LevelsDataService : ADataService<LevelsData>, ILevelsDataService
    {
        private GameplayEventProvider _gameplayEventProvider;
        public DataValue<LevelsData, int> CurrentLevel { get; private set; }
        public DataValue<LevelsData, int> LastPassedLevel { get; private set; }
        public DataValue<LevelsData, int> CollectionsUnlocked { get; }

        public int CurrentLevelUI => CurrentLevel.Value + 1;

        public LevelsDataService(IDatabase database, GameplayEventProvider gameplayEventProvider,
            MenuEventsProvider menuEventsProvider) : base(database)
        {
            _gameplayEventProvider = gameplayEventProvider;

            CurrentLevel = CreateValue(
                data => data.CurrentLevel,
                (data, value) => data.CurrentLevel = value,
                OnCurrentLevelChanged);

            LastPassedLevel = CreateValue(
                data => data.LevelsPass,
                (data, value) => data.LevelsPass = value);

            CollectionsUnlocked = CreateIntValue(
                data => data.CollectionsUnlocked,
                (data, value) => data.CollectionsUnlocked = value,
                menuEventsProvider.CollectionUnlockedEvent.Invoke);
        }

        public void ActualizeLevelsData()
        {
            CurrentLevel.Set(LastPassedLevel.Value + 1);
            TryToSave(true);
        }

        private void OnCurrentLevelChanged(int level)
        {
            _gameplayEventProvider.LevelSwitchEvent.Invoke(CurrentLevelUI);
        }
    }
}