using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Platforms
{
    [ConfigCategory(ConfigCategory.Game)]
    public class PlatformsConfigProvider : ScriptableConfig
    {
        [SerializeField, MinValue(0)]
        private float _platformSpacing = 4f;

        [SerializeField, MinValue(1)]
        private int _platformCountOnScreen = 3;

        [SerializeField, MinValue(1)]
        private float _platformWidth = 3;

        [SerializeField, PropertyRange(0, 100), SuffixLabel("%")]
        private float _jumpOnPlaceMovePercent = 20;

        [SerializeField]
        private ColorsConfig _colorsConfig;

        [SerializeField, MinValue(0)]
        private float _moveDuration = 0.3f;

        public float PlatformSpacing => _platformSpacing;
        public float JumpOnPlaceMovePercent => _jumpOnPlaceMovePercent;
        public ColorsConfig ColorsConfig => _colorsConfig;
        public float MoveDuration => _moveDuration;
        public int PlatformCountOnScreen => _platformCountOnScreen;
        public float PlatformWidth => _platformWidth;
    }
}