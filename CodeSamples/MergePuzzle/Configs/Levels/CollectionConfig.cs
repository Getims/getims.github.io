using System;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Levels
{
    [Serializable]
    [ConfigCategory(ConfigCategory.UI)]
    public class CollectionConfig : ScriptableConfig
    {
        [SerializeField]
        [PreviewField(ObjectFieldAlignment.Left, Height = 80)]
        private Sprite _sprite;

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _info;

        public Sprite Sprite => _sprite;
        public string Name => _name;
        public string Info => _info;
    }
}