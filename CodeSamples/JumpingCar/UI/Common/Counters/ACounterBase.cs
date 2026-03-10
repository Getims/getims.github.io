using System;
using DG.Tweening;
using Project.Scripts.Infrastructure.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Common.Counters
{
    public abstract class ACounterBase : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshProUGUI _valueTMP;

        [SerializeField]
        private float _updateDuration = 0.2f;

        [SerializeField]
        protected Button _button;

        private long _currentValue = -1;
        private Tweener _updateTw;

        public event Action OnCounterClick;

        public void UpdateInfo(long value = 0, bool instant = false, float updateDuration = -1)
        {
            if (updateDuration >= 0)
                _updateDuration = updateDuration;

            if (instant)
            {
                _updateTw?.Kill();
                _currentValue = value;
                _valueTMP.text = FormatValue(_currentValue);
            }
            else
                UpdateInfo(value);
        }

        protected virtual void Start()
        {
            if (_button != null)
                _button.onClick.AddListener(() => OnCounterClick?.Invoke());
            SubscribeToEvents();
            InitValue();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
            _updateTw?.Kill();
        }

        protected abstract void InitValue();
        protected abstract void SubscribeToEvents();
        protected abstract void UnsubscribeFromEvents();
        protected virtual MoneyConverter.ShortValueMode GetShortValueMode() => MoneyConverter.ShortValueMode.All;
        protected virtual bool CanGoDown() => false;

        protected void UpdateInfo(long value = 0)
        {
            _updateTw?.Kill();
            if (_updateDuration <= 0.01 || _currentValue < 0)
            {
                _currentValue = value;
                _valueTMP.text = FormatValue(_currentValue);
                return;
            }

            long startValue = _currentValue;
            long difference = value - _currentValue;
            float lerp = 0;
            _updateTw = DOTween.To(() => lerp, x => lerp = x, 1, _updateDuration)
                .OnUpdate(() =>
                {
                    long lerpValue = (long)(difference * lerp);
                    _currentValue = startValue + lerpValue;
                    if (_currentValue > value && !CanGoDown())
                        _currentValue = value;

                    _valueTMP.text = FormatValue(_currentValue);
                })
                .SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Fixed)
                .OnComplete(() =>
                {
                    _currentValue = value;
                    _valueTMP.text = FormatValue(_currentValue);
                });
        }

        protected virtual string FormatValue(long value)
        {
            return MoneyConverter.ConvertToShortValue(value, GetShortValueMode());
        }
    }
}