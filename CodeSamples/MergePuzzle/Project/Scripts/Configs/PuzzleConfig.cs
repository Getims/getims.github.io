using Project.Scripts.Core.Constants;
using Project.Scripts.Infrastructure.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs
{
    [ConfigCategory(ConfigCategory.Game)]
    public class PuzzleConfig : ScriptableConfig
    {
        [SerializeField, MinValue(0)]
        private float _elementPlaceDuration = 0.25f;

        [SerializeField, MinValue(1)]
        private float _dragScaleMultiplier = 1.1f;

        [SerializeField, MinValue(0)]
        private float _dragScaleTime = 0.2f;

        [SerializeField, MinValue(0)]
        private int _hintsCost = 100;

        [SerializeField]
        private int _hintsCountPerBuy = 5;

        public float ElementPlaceDuration => _elementPlaceDuration;
        public float DragScaleMultiplier => _dragScaleMultiplier;
        public int HintsCost => _hintsCost;
        public int HintsCountPerBuy => _hintsCountPerBuy;

        public float DragScaleTime => _dragScaleTime;
    }
}