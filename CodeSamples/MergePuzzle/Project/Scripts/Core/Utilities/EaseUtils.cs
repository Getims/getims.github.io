using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Core.Utilities
{
    public static partial class Utils
    {
        public static EaseFunction MakeOutSineThenLinear(float split = 0.5f)
        {
            split = Mathf.Clamp01(split);

            return (time, duration, overshootOrAmplitude, period) =>
            {
                float t = duration <= 0f ? 1f : Mathf.Clamp01(time / duration);

                if (t <= split)
                {
                    // Старт как OutSine
                    return Mathf.Sin(t * Mathf.PI * 0.5f);
                }
                else
                {
                    // Линейное продолжение от y(split) до 1 за оставшееся время
                    float ys = Mathf.Sin(split * Mathf.PI * 0.5f); // значение в точке перехода
                    float u = (t - split) / (1f - split); // нормализуем оставшийся интервал [split..1] -> [0..1]
                    return ys + u * (1f - ys); // линейно от ys до 1
                }
            };
        }
    }
}