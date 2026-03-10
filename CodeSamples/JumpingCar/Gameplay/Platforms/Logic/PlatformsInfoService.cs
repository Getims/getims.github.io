using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Data;
using Project.Scripts.Runtime.Constants;

namespace Project.Scripts.Gameplay.Platforms.Logic
{
    public interface IPlatformsInfoService
    {
        int CurrentPlatform { get; }

        void Initialize();
        string GetInfo(int platformNumber);
        void SetCurrentPlatform(int number);
    }

    public class PlatformsInfoService : IPlatformsInfoService
    {
        private readonly IGameDataService _gameDataService;
        private readonly Dictionary<int, string> _platformInfos = new();
        private int _currentPlatform = 0;

        public int CurrentPlatform => _currentPlatform;

        public PlatformsInfoService(IGameDataService gameDataService)
        {
            _gameDataService = gameDataService;
        }

        public void Initialize()
        {
            _platformInfos.Clear();
            var gameScores = _gameDataService.GameScores;

            if (gameScores.Count == 0) return;

            var maxScore = gameScores.Max();
            var avgScore = (int)gameScores.Average();
            var lastScore = gameScores.Last();
            var popularScore = gameScores.GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;

            _platformInfos[popularScore] = PlatformInfo.POPULAR_PLATFORM;
            _platformInfos[avgScore] = PlatformInfo.AVG_PLATFORM;
            _platformInfos[lastScore] = PlatformInfo.LAST_PLATFORM;
            _platformInfos[maxScore] = PlatformInfo.MAX_PLATFORM;
        }

        public string GetInfo(int platformNumber)
        {
            return _platformInfos.GetValueOrDefault(platformNumber);
        }

        public void SetCurrentPlatform(int number)
        {
            _currentPlatform = number;
        }
    }
}