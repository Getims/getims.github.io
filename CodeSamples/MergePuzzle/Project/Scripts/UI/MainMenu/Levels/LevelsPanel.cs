using System;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Core.Utilities;
using Project.Scripts.Data;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.UI.Common.Panels;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.MainMenu.Levels
{
    public class LevelsPanel : UIPanel
    {
        [SerializeField]
        private ButtonsInitializer _buttonsInitializer;

        [SerializeField, MinValue(0)]
        private float _animationTime = 0.25f;

        [Inject] private ILevelsDataService _levelsDataService;
        [Inject] private IConfigsProvider _configsProvider;

        public event Action OnAnimationCompleteEvent;

        private void Start()
        {
            var levelsConfigProvider = _configsProvider.GetConfig<LevelsConfigProvider>();
            _buttonsInitializer.Initialize(levelsConfigProvider, _levelsDataService.CurrentLevel.Value);
            UpdateButtons(_levelsDataService.CurrentLevel.Value);
        }

        public void UpdateButtons(int currentLevel, bool animate = false)
        {
            _buttonsInitializer.UpdateButtonsState(currentLevel);

            if (animate)
            {
                Utils.PerformWithDelay(this, _animationTime, () => OnAnimationCompleteEvent?.Invoke());
            }
        }
    }
}