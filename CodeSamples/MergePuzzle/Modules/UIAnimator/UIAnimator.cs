using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Animation = Project.Scripts.Modules.UIAnimator.Animations.Animation;

namespace Project.Scripts.Modules.UIAnimator
{
    public class UIAnimator : MonoBehaviour
    {
        [SerializeField]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "@ElementName", Expanded = true)]
        private List<Animation> _animations = new List<Animation>();

        [SerializeField]
        private bool _isLooped = false;

        [SerializeField]
        private bool _playOnAwake = false;

        private Sequence _sequence;

        public event Action OnComplete;

        [Button]
        public void Play()
        {
            _sequence.Kill();
            _sequence = BuildSequence();

            _sequence.SetLoops(_isLooped ? -1 : 1)
                .Play()
                .SetLink(gameObject)
                .OnComplete(OnAnimationComplete);
        }

        public void ForceStop()
        {
            Stop(false, false);
        }

        public void Stop(bool complete = false, bool completeWithCallbacks = false)
        {
            if (complete)
                _sequence.Complete(completeWithCallbacks);
            else
                _sequence.Kill();
        }

        private void Awake()
        {
            if (_playOnAwake)
                Play();
        }

        private void OnDestroy() =>
            Stop();

        private void OnEnable()
        {
            if (_playOnAwake)
                Play();
        }

        private void OnDisable()
        {
            Stop();
        }

        private Sequence BuildSequence()
        {
            Sequence sequence = DOTween.Sequence();
            foreach (Animation animation in _animations)
                animation.GetAction(ref sequence);
            return sequence;
        }

        private void OnAnimationComplete() =>
            OnComplete?.Invoke();

        [Button("Stop")]
        private void EditorStop() => Stop();

        [Button("Complete")]
        private void EditorComplete() => Stop();
    }

    internal enum AnimationType
    {
        Move,
        Rotate,
        Scale,
        Fade,
        Set,
        Action
    }

    internal enum PlayType
    {
        Append,
        Join
    }
}