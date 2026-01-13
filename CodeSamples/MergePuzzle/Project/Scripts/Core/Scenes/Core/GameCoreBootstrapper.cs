using System;
using Project.Scripts.Core.Events;
using Project.Scripts.Core.Scenes.Loader;
using Project.Scripts.Core.Utilities;
using Project.Scripts.Infrastructure.Services;
using Project.Scripts.Infrastructure.StateMachines;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Core.Scenes.Core
{
    public class GameCoreBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [Inject] private IGameStateMachine _gameStateMachine;
        [Inject] private StateMachineFactory _stateMachineFactory;
        [Inject] private GlobalEventProvider _globalEventProvider;

        private Enums.Scenes _lastScene = Enums.Scenes.GameLoader;

        private void Start()
        {
            _globalEventProvider.TryToSwitchSceneEvent.AddListener(TryToSwitchScene);

            SetupFramerate();
            SetupGameStates();
            EnterBootstrapState();
        }

        private void SetupGameStates()
        {
            _stateMachineFactory.BindState<LoadGameLoaderSceneState>(_gameStateMachine);
            _stateMachineFactory.BindState<GameLoaderState>(_gameStateMachine);

            _stateMachineFactory.BindState<LoadGameSceneState>(_gameStateMachine);
            _stateMachineFactory.BindState<LoadMainMenuState>(_gameStateMachine);
            _stateMachineFactory.BindState<GameLoopState>(_gameStateMachine);
        }

        private void SetupFramerate() =>
            FramerateSetter.SetDefaultFramerate();

        private void EnterBootstrapState() =>
            _gameStateMachine.Enter<LoadGameLoaderSceneState>();

        private void TryToSwitchScene(Enums.Scenes gameState)
        {
            switch (gameState)
            {
                case Enums.Scenes.NULL:
                    if (_lastScene != Enums.Scenes.NULL)
                        TryToSwitchScene(_lastScene);
                    else
                        TryToSwitchScene(Enums.Scenes.GameLoader);
                    break;
                case Enums.Scenes.GameLoader:
                    _gameStateMachine.Enter<LoadGameLoaderSceneState>();
                    break;
                case Enums.Scenes.MainMenu:
                    _gameStateMachine.Enter<LoadMainMenuState>();
                    break;
                case Enums.Scenes.Game:
                    _gameStateMachine.Enter<LoadGameSceneState>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
            }

            _lastScene = gameState;
        }
    }
}