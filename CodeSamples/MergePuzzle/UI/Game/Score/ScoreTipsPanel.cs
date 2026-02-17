using Coffee.UIExtensions;
using Project.Scripts.Configs.Combo;
using Project.Scripts.Gameplay.GameFlow;
using Project.Scripts.Runtime.Audio;
using Project.Scripts.Runtime.Events;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Game.Score
{
    public class ScoreTipsPanel : UIPanel
    {
        [SerializeField]
        private Transform _basePosition;

        [SerializeField]
        private ScoreTipsPool _comboPool;

        [SerializeField]
        private UIParticle _comboParticles;

        [Inject] private GameplayEventProvider _gameplayEventProvider;
        [Inject] private ISoundService _soundService;
        [Inject] private IGameFlowController _gameFlowController;

        private void Start()
        {
            _gameplayEventProvider.ComboTriggeredEvent.AddListener(OnComboTriggered);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _gameplayEventProvider.ComboTriggeredEvent.RemoveListener(OnComboTriggered);
        }

        private void ShowComboParticles()
        {
            if (_comboParticles == null)
                return;

            _comboParticles.Play();
        }

        private void OnComboTriggered(ComboConfig comboConfig)
        {
            if (comboConfig == null)
                return;

            _soundService.PlayComboSound(comboConfig.ComboType);

            ScoreTip comboTip = _comboPool.GetScoreTip();
            comboTip.Initialize(comboConfig.Sprite);
            comboTip.SetPosition(_basePosition.position);
            comboTip.Show();

            ShowComboParticles();
        }
    }
}