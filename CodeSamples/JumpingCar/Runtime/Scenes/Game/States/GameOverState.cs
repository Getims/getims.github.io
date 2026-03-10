using Project.Scripts.Data;
using Project.Scripts.Gameplay.Platforms.Logic;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Runtime.Audio;
using Project.Scripts.UI.Game;
using Utils = Project.Scripts.Infrastructure.Utilities.Utils;

namespace Project.Scripts.Runtime.Scenes.Game.States
{
    public class GameOverState : IEnterState, IExitState
    {
        private readonly GameStateMachine _stateMachine;

        private LevelCompletePanel _levelCompletePanel;
        private IGameUIController _gameUIController;
        private IGameDataService _gameDataService;
        private IPlatformsInfoService _platformsInfoService;
        private ISoundService _soundService;

        public GameOverState(GameStateMachine stateMachine, IGameUIController gameUIController,
            IGameDataService gameDataService, IPlatformsInfoService platformsInfoService, ISoundService soundService)
        {
            _soundService = soundService;
            _platformsInfoService = platformsInfoService;
            _gameDataService = gameDataService;
            _gameUIController = gameUIController;
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            SaveData();
            HideGameHud();
            CreateLevelCompletePanel();
        }

        public void Exit()
        {
            if (_levelCompletePanel != null)
                _levelCompletePanel.OnClaimClick -= OnLevelComplete;
        }

        private void SaveData()
        {
            _gameDataService.AddGameResult(_platformsInfoService.CurrentPlatform);
        }

        private void HideGameHud()
        {
            _gameUIController.GetPanel<TopGamePanel>().Hide();
            _gameUIController.GetPanel<InputPanel>().Hide();
        }

        private void CreateLevelCompletePanel()
        {
            if (_levelCompletePanel != null)
                return;

            _levelCompletePanel = _gameUIController.GetPanel<LevelCompletePanel>();
            _levelCompletePanel.OnClaimClick += OnLevelComplete;
            _levelCompletePanel.Show();
            Utils.PerformWithDelay(_levelCompletePanel, _levelCompletePanel.FadeTime, () =>
                _soundService.PlayFailSound());
        }

        private void OnLevelComplete()
        {
            _stateMachine.Enter<ResetGameState>();
        }
    }
}