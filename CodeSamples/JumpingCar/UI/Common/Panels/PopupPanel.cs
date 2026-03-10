using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Common.Panels
{
    public class PopupPanel : UIPanel
    {
        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private Button _claimButton;

        public event Action OnCloseClick;
        public event Action OnClaimClick;

        protected virtual void Start()
        {
            if (_closeButton != null)
                _closeButton.onClick.AddListener(OnCloseButtonClick);

            if (_claimButton != null)
                _claimButton.onClick.AddListener(OnClaimButtonClick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopAllCoroutines();

            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(OnCloseButtonClick);

            if (_claimButton != null)
                _claimButton.onClick.RemoveListener(OnClaimButtonClick);
        }

        protected virtual void OnCloseButtonClick()
        {
            Hide();
            OnCloseClick?.Invoke();
        }

        protected virtual void OnClaimButtonClick()
        {
            Hide();
            OnClaimClick?.Invoke();
        }
    }
}