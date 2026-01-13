using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Common.Controls
{
    public abstract class AControlButton<T> : MonoBehaviour
    {
        [SerializeField]
        protected Button _button;

        [SerializeField]
        protected GameObject _activeState;

        [SerializeField]
        protected T _buttonType;

        public T ButtonType => _buttonType;

        protected Action<T> _onClick;
        public bool IsActive => gameObject.activeSelf || gameObject.activeInHierarchy;

        public virtual void Initialize(Action<T> onClick)
        {
            _onClick = onClick;
        }

        public virtual void SetSelected(bool isSelected)
        {
            if (_activeState != null)
                _activeState.SetActive(isSelected);
        }

        public virtual void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        protected void Start()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        protected void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        protected virtual void OnButtonClick()
        {
            _onClick?.Invoke(_buttonType);
        }
    }
}