using System.Collections.Generic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Background
{
    [ConfigCategory(ConfigCategory.Game)]
    public class BackgroundConfigProvider : ScriptableConfig
    {
        [SerializeField, PropertyRange(0, 1)]
        private float _backgroundParallaxFactor = 0.2f;

        [SerializeField]
        private List<Sprite> _backgroundGradients = new List<Sprite>();

        public float BackgroundParallaxFactor => _backgroundParallaxFactor;
        public IReadOnlyCollection<Sprite> BackgroundGradients => _backgroundGradients;
    }
}