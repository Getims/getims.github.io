using Project.Scripts.Configs.Levels;
using Project.Scripts.Data;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Audio;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.Runtime.Enums;
using Project.Scripts.Runtime.Events;
using Project.Scripts.UI.Common;
using Project.Scripts.UI.Common.Anchors;
using Project.Scripts.UI.Common.FlyIcons;
using Project.Scripts.UI.MainMenu.Collections;
using Project.Scripts.UI.MainMenu.Levels;
using Project.Scripts.UI.MainMenu.Main;
using Project.Scripts.UI.MainMenu.Settings;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Utils = Project.Scripts.Infrastructure.Utilities.Utils;

namespace Project.Scripts.UI.MainMenu
{
    public class MainMenuUIController : AUIController
    {
        [SerializeField]
        private FlyIconsSpawner _flyIconsSpawner;

        [Inject] private ISoundService _soundService;
        [Inject] private GlobalEventProvider _globalEventProvider;
        [Inject] private ICurrencyDataService _currencyDataService;
        [Inject] private ILevelsDataService _levelsDataService;
        [Inject] private IUIAnchorsProvider _uiAnchorsProvider;
        [Inject] private MenuEventsProvider _menuEventsProvider;
        [Inject] private IConfigsProvider _configsProvider;

        private LevelsPanel _levelsPanel;
        private CollectionPanel _collectionPanel;

        public override void Initialize()
        {
            base.Initialize();

            ShowMainPanel();
            ShowLevelsPanel();
            Utils.PerformWithDelay(this, GameConstants.MAIN_MENU_ANIMATION_DELAY, StartAnimations);
        }

        protected override void Start()
        {
            Initialize();
            _menuEventsProvider.MoneySpawnRequest.AddListener(OnMoneySpawnRequest);
            _menuEventsProvider.CollectionUnlockedEvent.AddListener(OnCollectionUnlockedEvent);
        }

        protected override void OnDestroy()
        {
            _menuEventsProvider.MoneySpawnRequest.RemoveListener(OnMoneySpawnRequest);
            _menuEventsProvider.CollectionUnlockedEvent.RemoveListener(OnCollectionUnlockedEvent);
        }

        private void ShowMainPanel()
        {
            var panel = ShowPanel<MenuPanel>();
            panel.OnCollectionOpenRequest += ShowCollectionPanel;
            panel.OnSettingsOpenRequest += ShowSettingsPopup;
            panel.OnStartLevelOpenRequest += ShowStartLevelPopup;
        }

        private void ShowCollectionPanel()
        {
            _collectionPanel = ShowPanel<CollectionPanel>();
        }

        private void ShowLevelsPanel()
        {
            if (_levelsPanel == null)
                _levelsPanel = ShowPanel<LevelsPanel>();

            _levelsPanel.Initialize();
            _levelsPanel.Show();
        }

        private void ShowStartLevelPopup()
        {
            StartLevel();
        }

        private void ShowSettingsPopup()
        {
            OpenPopup<MainMenuSettingsPopup>(CloseAllPopups, null);
        }

        private void StartLevel()
        {
            _globalEventProvider.TryToSwitchSceneEvent.Invoke(Scenes.Game);
        }

        private void ActualizeData()
        {
            _levelsDataService.ActualizeLevelsData();
        }

        private void StartAnimations()
        {
            _levelsPanel.UpdateButtons(_levelsDataService.LastPassedLevel.Value + 1, true);
            _levelsPanel.OnAnimationCompleteEvent += OnAnimationComplete;
        }

        [Button]
        private void ShowMoneyReward(Vector3 spawnPosition)
        {
            _flyIconsSpawner.StartAnimation(spawnPosition,
                _uiAnchorsProvider.GetAnchorPosition(UIAnchorType.MoneyCounter));

            var animationDuration = _flyIconsSpawner.GetAnimationTime();
            Utils.PerformWithDelay(this, animationDuration, AddRewardToMoney);
        }

        private void AddRewardToMoney()
        {
            var reward = _currencyDataService.PendingReward.Value;
            _currencyDataService.Money.Add(reward);
            _currencyDataService.PendingReward.Set(0);
        }

        private void OnStartLevelClaim()
        {
            StartLevel();
        }

        private void OnAnimationComplete()
        {
            ActualizeData();
        }

        private void OnMoneySpawnRequest(Vector3 spawnPosition)
        {
            ShowMoneyReward(spawnPosition);
        }

        [Button]
        private void OnCollectionUnlockedEvent(int collectionsCount)
        {
            var collectionPreview = OpenPopup<CollectionPreview>(() => { }, () => { });
            collectionPreview.HideEvent += () => collectionPreview.DestroySelf();
            var levelsConfigProvider = _configsProvider.GetConfig<LevelsConfigProvider>();

            collectionPreview.SetConfig(levelsConfigProvider.GetCollectionConfig(collectionsCount - 1));
            collectionPreview.Show();
        }
    }
}