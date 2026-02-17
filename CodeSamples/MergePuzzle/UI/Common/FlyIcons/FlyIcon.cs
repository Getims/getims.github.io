using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Common.FlyIcons
{
    public class FlyIcon : MonoBehaviour
    {
        [SerializeField]
        private Image _iconImg;

        [SerializeField]
        private RectTransform _iconRT;

        [SerializeField]
        private RectTransform _iconScaleRT;

        [SerializeField]
        private CanvasGroup _iconCG;

        private Sequence _flySequence;
        private FlySettings _flySettings = new FlySettings();
        private Action _onComplete;
        private Action _onMoved;


        public void Setup(Sprite sprite, FlySettings flySettings, Action onMoved = null, Action onComplete = null)
        {
            Kill();

            if (_iconScaleRT == null)
                _iconScaleRT = _iconRT;

            _iconImg.sprite = sprite;
            _flySettings = flySettings;
            _onMoved = onMoved;
            _onComplete = onComplete;

            _iconCG.alpha = _flySettings.ShowTime > 0 ? 0 : 1;
            if (_flySettings.ScaleFromZeroTime > 0)
                _iconScaleRT.localScale = Vector3.zero;
            else
                _iconScaleRT.localScale = _flySettings.StartScale * Vector3.one;

            _iconRT.anchoredPosition = new Vector2(0, 0);
        }

        public void PlayAnimation(Vector3 startPosition, Vector3 targetPosition, SequenceBuilder customBuilder = null)
        {
            Kill();

            SequenceBuilder sequenceBuilder = new SequenceBuilder();
            if (customBuilder != null)
                sequenceBuilder = customBuilder;
            SequenceConfig sequenceConfig = new SequenceConfig(_iconRT, _iconScaleRT, _iconCG, _flySettings, OnMoved);
            _flySequence = sequenceBuilder.BuildSequence(startPosition, targetPosition, sequenceConfig);

            _flySequence.OnComplete(() => _onComplete?.Invoke());
            _flySequence.Play();
        }

        protected virtual void OnMoved() => _onMoved?.Invoke();
        private void OnDestroy() => Kill();
        private void Kill() => _flySequence?.Kill();
    }
}