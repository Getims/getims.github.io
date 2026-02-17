using System;
using Project.Scripts.Gameplay.Levels;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.GameFlow
{
    public interface IGameFlowController
    {
        bool IsLoadComplete { get; }

        void Initialize();
        void GenerateLevel();
        void SetGameOver(bool isWin);
        event Action<bool> OnGameOver;
    }

    public class GameFlowController : MonoBehaviour, IGameFlowController
    {
        [SerializeField]
        private LevelController _levelController;

        [SerializeField]
        private BackgroundController _backgroundController;

        [Inject] private GlobalEventProvider _globalEventProvider;
        [Inject] private IConfigsProvider _configsProvider;

        private bool _isGameComplete = false;
        private bool _isLoadComplete = false;

        public bool IsLoadComplete => _isLoadComplete;
        public event Action<bool> OnGameOver;

        public void Initialize()
        {
            StaticInputService.SetDragBlock(true);
            _levelController.Initialize();
            _levelController.OnLevelCompleteEvent += () => SetGameOver(true);
            _backgroundController.Initialize();
        }

        [Button]
        public void GenerateLevel()
        {
            _levelController.GenerateLevel();
            _isGameComplete = false;
            StaticInputService.SetDragBlock(false);
        }

        public void SetGameOver(bool isWin)
        {
            if (_isGameComplete)
                return;

            _levelController.ShowResult();
            _levelController.ClearLevel();
            _backgroundController.SetWinBackground();
            _isGameComplete = isWin;
            StaticInputService.SetDragBlock(false);
            OnGameOver?.Invoke(isWin);
        }

        private void Start()
        {
            _isLoadComplete = true;
        }
    }
}