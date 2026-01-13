using Project.Scripts.Configs;
using Project.Scripts.Core.Enums;
using Project.Scripts.Core.Events;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.UI.Common.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.DebugModule
{
    public class DevConsole : UIPanel
    {
        [SerializeField]
        protected TMP_Text _buildVersionTMP;

        [SerializeField]
        protected Button _openButton;

        [SerializeField]
        protected Button _closeButton;

        [SerializeField]
        protected Button _restartButton;

        [SerializeField]
        private Scenes _scene = Scenes.MainMenu;

        private GlobalEventProvider _globalEventProvider;
        private bool _enableDebug;

        [Inject]
        public void Construct(GlobalEventProvider globalEventProvider, IConfigsProvider configsProvider)
        {
            _globalEventProvider = globalEventProvider;
            _enableDebug = configsProvider.GetConfig<GlobalConfig>().EnableDebug;
        }

        protected virtual void Start()
        {
            gameObject.SetActive(_enableDebug);

            _openButton.onClick.AddListener(Show);
            _closeButton.onClick.AddListener(Hide);
            _restartButton.onClick.AddListener(HotRestart);

            _buildVersionTMP.text = $"Version {Application.version}";
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Time.timeScale = 1;
        }

        private void HotRestart() =>
            _globalEventProvider.TryToSwitchSceneEvent.Invoke(_scene);
    }
}