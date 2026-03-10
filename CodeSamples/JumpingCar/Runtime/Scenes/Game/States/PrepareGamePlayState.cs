using System.Collections;
using Project.Scripts.Gameplay.GameFlow.Logic;
using Project.Scripts.Infrastructure.Services;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Audio;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.UI.Game;
using UnityEngine;

namespace Project.Scripts.Runtime.Scenes.Game.States
{
    public class PrepareGamePlayState : IEnterState, IExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IGameFlow _gameFlowController;
        private readonly IGameUIController _gameUIController;

        private Coroutine _levelCreationCO;
        private ISoundService _soundService;

        public PrepareGamePlayState(GameStateMachine stateMachine, ICoroutineRunner coroutineRunner,
            IGameFlow gameFlowController, IGameUIController gameUIController, ISoundService soundService)
        {
            _soundService = soundService;
            _gameUIController = gameUIController;
            _gameFlowController = gameFlowController;
            _coroutineRunner = coroutineRunner;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            if (_levelCreationCO != null)
                _coroutineRunner.StopCoroutine(_levelCreationCO);

            _levelCreationCO = _coroutineRunner.StartCoroutine(CreateLevel());
        }

        public void Exit()
        {
            if (_levelCreationCO != null)
                _coroutineRunner?.StopCoroutine(_levelCreationCO);

            var prestartPanel = _gameUIController.GetPanel<MainMenuPanel>();
            prestartPanel.OnStartClick -= StartLevel;
            prestartPanel.Hide();
        }

        private IEnumerator CreateLevel()
        {
            _soundService.PlayGameBackgroundMusic();

            yield return new WaitForSeconds(GameConstants.SCENE_LOAD_TIME);
            _gameFlowController.Initialize();

            yield return new WaitForEndOfFrame();
            _gameUIController.Initialize();

            while (_gameFlowController.IsLoadComplete == false)
                yield return new WaitForEndOfFrame();

            ShowPrestartPanel();
            yield return null;
        }

        private void ShowPrestartPanel()
        {
            var prestartPanel = _gameUIController.ShowPanel<MainMenuPanel>();
            prestartPanel.OnStartClick += StartLevel;
        }

        private void StartLevel()
        {
            _stateMachine.Enter<GamePlayState>();
        }
    }
}