using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.UI.Common.Controls
{
    public abstract class AControlPanel<T, TButton> : MonoBehaviour
        where TButton : AControlButton<T>
    {
        [SerializeField]
        protected List<TButton> _controlButtons = new List<TButton>();

        [SerializeField]
        protected int _firstActiveButton = 0;

        protected T _currentButton;

        public event Action<T> OnControlButtonClick;

        public virtual void Initialize()
        {
            foreach (TButton controlButton in _controlButtons)
                controlButton.Initialize(OnButtonClick);

            if (_firstActiveButton >= _controlButtons.Count)
                _firstActiveButton = 0;

            _controlButtons[_firstActiveButton].SetSelected(true);
            _currentButton = _controlButtons[_firstActiveButton].ButtonType;
        }

        public void ForceClick(T buttonType)
        {
            SetSelectedButton(buttonType);

            _currentButton = buttonType;
            OnControlButtonClick?.Invoke(buttonType);
        }

        protected void OnButtonClick(T buttonType)
        {
            SetSelectedButton(buttonType);

            _currentButton = buttonType;
            OnControlButtonClick?.Invoke(buttonType);
        }

        protected virtual void SetSelectedButton(T buttonType)
        {
            foreach (TButton controlButton in _controlButtons)
                controlButton.SetSelected(controlButton.ButtonType.Equals(buttonType));
        }
    }
}