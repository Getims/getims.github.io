using System;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Game
{
    public class PrestartPanel : UIPanel
    {
        [SerializeField]
        private Button _randomButton;

        [SerializeField]
        private Button _startButton;

        public event Action OnRandomClick;
        public event Action OnStartClick;

        protected void Start()
        {
            _randomButton.onClick.AddListener(OnRandomButtonClick);
            _startButton.onClick.AddListener(OnStartButtonClick);
        }

        protected override void OnDestroy()
        {
            _randomButton.onClick.RemoveListener(OnRandomButtonClick);
            _startButton.onClick.RemoveListener(OnStartButtonClick);
        }

        private void OnRandomButtonClick() => OnRandomClick?.Invoke();
        private void OnStartButtonClick() => OnStartClick?.Invoke();
    }
}