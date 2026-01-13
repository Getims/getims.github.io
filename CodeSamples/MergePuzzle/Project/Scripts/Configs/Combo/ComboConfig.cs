using System;
using Project.Scripts.Core.Constants;
using Project.Scripts.Core.Enums;
using Project.Scripts.Infrastructure.Configs;
using UnityEngine;

namespace Project.Scripts.Configs.Combo
{
    [Serializable]
    [ConfigCategory(ConfigCategory.Game)]
    public class ComboConfig : ScriptableConfig
    {
        [SerializeField]
        private ComboType _comboType;

        [SerializeField]
        private Sprite _sprite;

        public ComboType ComboType => _comboType;
        public Sprite Sprite => _sprite;
    }
}