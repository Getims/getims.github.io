using System;
using Project.Scripts.Gameplay.Factory;
using Project.Scripts.Gameplay.Units;

namespace Project.Scripts.Gameplay.GameFlow.Logic
{
    public interface IGameFlow
    {
        bool IsLoadComplete { get; }

        void Initialize();
        void GenerateUnits();
        void StartBattle();
        void SetGameOver();
        event Action OnGameOver;
    }

    public class GameFlow : IGameFlow
    {
        private readonly UnitsController _unitsController;
        private readonly UnitsFactoryController _unitsFactoryController;
        private readonly IGameInfoService _gameInfoService;

        private bool _isGameComplete = false;
        private bool _isLoadComplete = false;

        public bool IsLoadComplete => _isLoadComplete;
        public event Action OnGameOver;

        public GameFlow(UnitsController unitsController, UnitsFactoryController unitsFactoryController,
            IGameInfoService gameInfoService)
        {
            _unitsController = unitsController;
            _unitsFactoryController = unitsFactoryController;
            _gameInfoService = gameInfoService;

            _gameInfoService.OnOneTeamAlive += OnOneTeamAlive;
        }

        public void Initialize()
        {
            _unitsFactoryController.Initialize();
            _unitsController.Initialize(_unitsFactoryController);
            _isGameComplete = false;
            _isLoadComplete = true;
        }

        public void GenerateUnits() => _unitsController.CreateUnits();

        public void StartBattle() => _unitsController.StartBattle();

        public void SetGameOver()
        {
            if (_isGameComplete)
                return;

            _unitsController.StopBattle();
            _isGameComplete = true;
            OnGameOver?.Invoke();
        }

        private void OnOneTeamAlive() => SetGameOver();
    }
}