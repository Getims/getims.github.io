using System;
using Project.Scripts.Gameplay.Factory;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Gameplay.Units;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.GameFlow
{
    public class GameFlowController : MonoBehaviour, IGameFlow
    {
        [SerializeField]
        private UnitsController _unitsController;

        [SerializeField]
        private UnitsFactoryController _unitsFactoryController;

        [Inject] private IGameInfoService _gameInfoService;

        private Logic.GameFlow _gameFlow;

        public bool IsLoadComplete => _gameFlow.IsLoadComplete;

        public event Action OnGameOver
        {
            add => _gameFlow.OnGameOver += value;
            remove => _gameFlow.OnGameOver -= value;
        }

        private void Awake()
        {
            _gameFlow = new Logic.GameFlow(_unitsController, _unitsFactoryController, _gameInfoService);
        }

        public void Initialize() => _gameFlow.Initialize();
        public void GenerateUnits() => _gameFlow.GenerateUnits();
        public void StartBattle() => _gameFlow.StartBattle();
        public void SetGameOver() => _gameFlow.SetGameOver();
    }
}