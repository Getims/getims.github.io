using System.Collections;
using Project.Scripts.Data;
using Project.Scripts.Gameplay.GameFlow;
using Project.Scripts.Infrastructure.Services;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.UI.Common;
using Project.Scripts.UI.Game;
using Project.Scripts.UI.Game.Score;
using Project.Scripts.UI.Game.Settings;
using Project.Scripts.UI.Game.Top;
using UnityEngine;

namespace Project.Scripts.Runtime.Scenes.Game.States
{
    public class PrepareGamePlayState : IEnterState, IExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IUIFactory _iuiFactory;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly UIContainerProvider _uiContainerProvider;
        private readonly IGameFlowController _gameFlowController;

        private Coroutine _levelCreationCO;
        private TopGamePanel _topGamePanel;
        private ILevelsDataService _levelsDataService;
        private GameSettingsPopup _gameSettingsPopup;
        private ScoreTipsPanel _scoreTipsPanel;

        public PrepareGamePlayState(GameStateMachine stateMachine, IUIFactory iuiFactory,
            ICoroutineRunner coroutineRunner, UIContainerProvider uiContainerProvider,
            IGameFlowController gameFlowController, ILevelsDataService levelsDataService)
        {
            _levelsDataService = levelsDataService;
            _gameFlowController = gameFlowController;
            _uiContainerProvider = uiContainerProvider;
            _coroutineRunner = coroutineRunner;
            _iuiFactory = iuiFactory;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            if (_levelCreationCO != null)
                _coroutineRunner.StopCoroutine(_levelCreationCO);

            _levelCreationCO = _coroutineRunner.StartCoroutine(CreateLevel());

            if (_scoreTipsPanel == null)
                _scoreTipsPanel = _iuiFactory.GetPanel<ScoreTipsPanel>();
        }

        public void Exit()
        {
            if (_levelCreationCO != null)
                _coroutineRunner?.StopCoroutine(_levelCreationCO);
        }

        private void CreateTopPanel(bool show = true)
        {
            if (_topGamePanel == null)
            {
                _topGamePanel = _iuiFactory.Create<TopGamePanel>(_uiContainerProvider.MenuContainer);
                _topGamePanel.OnSettingsClick += ShowSettings;
                _topGamePanel.OnNoHintsClick += ShowNoHints;
            }

            if (show)
                _topGamePanel.Show();
        }


        private void CreateSettingsPopup(bool show = true)
        {
            if (_gameSettingsPopup == null)
            {
                _gameSettingsPopup = _iuiFactory.GetPanel<GameSettingsPopup>();
                _gameSettingsPopup.OnRestartClick += RestartLevel;
                _gameSettingsPopup.OnExitClick += ExitLevel;
            }

            if (show)
                _gameSettingsPopup.Show();
        }

        private void CreateHintsPanel()
        {
            var panel = _iuiFactory.GetPanel<HintsPopup>();
            panel.Initialize();
            panel.Show();
        }

        private void ShowSettings()
        {
            CreateSettingsPopup(true);
        }

        private void ShowNoHints()
        {
            CreateHintsPanel();
        }

        private void RestartLevel() =>
            _stateMachine.Enter<ResetGameState>();

        private void ExitLevel()
        {
            _stateMachine.Enter<ExitGameState>();
        }

        IEnumerator CreateLevel()
        {
            yield return new WaitForSeconds(GameConstants.SCENE_LOAD_TIME);
            _gameFlowController.Initialize();

            yield return new WaitForEndOfFrame();
            CreateTopPanel();

            while (_gameFlowController.IsLoadComplete == false)
                yield return new WaitForEndOfFrame();

            _gameFlowController.GenerateLevel();
            _stateMachine.Enter<GamePlayState>();
            yield return null;
        }
    }
}