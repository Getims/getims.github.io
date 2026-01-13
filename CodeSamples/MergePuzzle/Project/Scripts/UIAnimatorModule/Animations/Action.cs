using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Scripts.UIAnimatorModule.Animations
{
    [Serializable]
    public class Action
    {
        [SerializeField]
        private UnityEvent _event;

        public string GetObjectName()
        {
            if (_event.GetPersistentEventCount() != 0)
                return $"Action {_event.GetPersistentEventCount()}";
            else
                return string.Empty;
        }

        public Tween GetTween(float animationTime, bool instant)
        {
            return DOVirtual.DelayedCall(instant ? 0 : animationTime, () => _event?.Invoke());
        }
    }
}