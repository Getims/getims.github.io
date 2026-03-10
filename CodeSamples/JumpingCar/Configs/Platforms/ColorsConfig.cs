using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using UnityEngine;

namespace Project.Scripts.Configs.Platforms
{
    [ConfigCategory(ConfigCategory.Game)]
    public class ColorsConfig : ScriptableConfig
    {
        [SerializeField]
        private Color _currentPlatformColor;

        [SerializeField]
        private Color _nextPlatformColor;

        [SerializeField]
        private Color _infoPlatformColor;

        public Color CurrentPlatformColor => _currentPlatformColor;
        public Color NextPlatformColor => _nextPlatformColor;
        public Color InfoPlatformColor => _infoPlatformColor;
    }
}