using System.Collections.Generic;
using Project.Scripts.Infrastructure.Data;

namespace Project.Scripts.Data
{
    public interface IGameDataService
    {
        public IReadOnlyCollection<int> GameScores { get; }
        public void AddGameResult(int result);
    }

    public class GameDataService : ADataService<GameData>, IGameDataService
    {
        public IReadOnlyCollection<int> GameScores => _data.GameScores;

        public GameDataService(IDatabase database) : base(database)
        {
        }

        public void AddGameResult(int result)
        {
            if (result <= 0)
                return;

            _data.GameScores.Add(result);
            TryToSave(true);
        }
    }
}