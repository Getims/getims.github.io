using System;
using UnityEngine;

namespace Project.Scripts.UI.Common.FlyIcons
{
    public struct SequenceConfig
    {
        public RectTransform IconRT;
        public RectTransform IconScaleRT;
        public CanvasGroup IconCG;
        public FlySettings FlySettings;
        public Action OnMoved;

        public SequenceConfig(RectTransform iconRT, RectTransform iconScaleRT, CanvasGroup iconCg,
            FlySettings flySettings, Action onMoved)
        {
            IconRT = iconRT;
            IconScaleRT = iconScaleRT;
            IconCG = iconCg;
            FlySettings = flySettings;
            OnMoved = onMoved;
        }
    }
}