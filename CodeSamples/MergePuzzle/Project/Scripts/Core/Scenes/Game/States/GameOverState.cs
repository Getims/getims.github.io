using Project.Scripts.Configs.Levels;
using Project.Scripts.Data;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Infrastructure.StateMachines;
using Project.Scripts.Infrastructure.StateMachines.States;
using Project.Scripts.UI.Common;
using Project.Scripts.UI.Game;
using Project.Scripts.UI.Game.Settings;
using Project.Scripts.UI.Game.Top;

namespace Project.Scripts.Core.Scenes.Game.States
{
    public class GameOverState : IEnterState<bool>, IExitState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IUIFactory _uiMenuFactory;
        private readonly UIContainerProvider _uiContainerProvider;
        private readonly ILevelsDataService _levelsDataService;
        private readonly LevelsConfigProvider _gameLevelsConfigProvider;
        private readonly ICurrencyDataService _currencyDataService;

        private LevelCompletePanel _levelCompletePanel;

        public GameOverState(GameStateMachine stateMachine, IUIFactory uiMenuFactory,
            UIContainerProvider uiContainerProvider, ILevelsDataService levelsDataService,
            IConfigsProvider configsProvider, ICurrencyDataService currencyDataService)
        {
            _gameLevelsConfigProvider = configsProvider.GetConfig<LevelsConfigProvider>();
            _uiContainerProvider = uiContainerProvider;
            _uiMenuFactory = uiMenuFactory;
            _stateMachine = stateMachine;
            _levelsDataService = levelsDataService;
            _currencyDataService = currencyDataService;
        }

        public void Enter(bool isWin)
        {
            if (isWin)
            {
                CreateLevelCompletePanel();
                HideGameHud();
            }
        }

        public void Exit()
        {
        }

        private void HideGameHud()
        {
            _uiMenuFactory.GetPanel<TopGamePanel>().Hide();
            _uiMenuFactory.GetPanel<GameSettingsPopup>().Hide();
        }

        private void CreateLevelCompletePanel()
        {
            if (_levelCompletePanel != null)
                return;

            _levelCompletePanel = _uiMenuFactory.Create<LevelCompletePanel>(_uiContainerProvider.WindowsContainer);
            _levelCompletePanel.OnClaimClick += OnLevelComplete;
            _levelCompletePanel.Show();
        }

        private void SaveReward()
        {
            int currentLevel = _levelsDataService.CurrentLevel.Value;
            LevelConfig levelConfig = _gameLevelsConfigProvider.GetLevel(currentLevel);
            _currencyDataService.PendingReward.Set(levelConfig.Reward);
        }

        private void LoadNextLevel()
        {
            _levelsDataService.LastPassedLevel.Set(_levelsDataService.CurrentLevel.Value);
        }

        private void OnLevelComplete()
        {
            SaveReward();
            LoadNextLevel();

            _stateMachine.Enter<ExitGameState>();
        }
    }
}