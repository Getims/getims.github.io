using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.UI.Common.FlyIcons
{
    public class SequenceBuilder
    {
        public virtual Sequence BuildSequence(Vector3 startPosition, Vector3 targetPosition, SequenceConfig config)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(config.IconScaleRT
                .DOScale(Vector3.one * config.FlySettings.StartScale, config.FlySettings.ScaleFromZeroTime)
                .SetEase(config.FlySettings.ScaleEase));
            sequence.Join(config.IconCG.DOFade(1, config.FlySettings.ShowTime)
                .SetDelay(config.FlySettings.ShowDelay));

            config.IconRT.position = startPosition;
            float moveTime = config.FlySettings.MoveTime;
            moveTime += Random.Range(0, config.FlySettings.ExtraMoveTimeRandom);

            sequence.Append(config.IconRT.DOMove(targetPosition, moveTime)
                .SetEase(config.FlySettings.MoveEase)
                .SetDelay(config.FlySettings.MoveDelay)
                .OnComplete(config.OnMoved.Invoke)
            );

            sequence.Append(config.IconScaleRT
                .DOScale(Vector3.one * config.FlySettings.EndScale, config.FlySettings.ScaleTime)
                .SetEase(config.FlySettings.ScaleEase)
                .SetDelay(config.FlySettings.ScaleDelay));

            sequence.Join(config.IconCG.DOFade(0, config.FlySettings.HideTime)
                .SetDelay(config.FlySettings.HideDelay));

            return sequence;
        }

        public virtual float GetSequenceDuration(FlySettings config)
        {
            float scaleFromZeroTime = config.ScaleFromZeroTime;
            float showTime = config.ShowTime + config.ShowDelay;

            float moveTime = config.MoveTime + config.MoveDelay + config.ExtraMoveTimeRandom;

            float scaleTime = config.ScaleTime + config.ScaleDelay;
            float hideTime = config.HideTime + config.HideDelay;

            float result = Mathf.Max(scaleFromZeroTime, showTime) + moveTime + Mathf.Max(scaleTime, hideTime);

            return result;
        }
    }
}