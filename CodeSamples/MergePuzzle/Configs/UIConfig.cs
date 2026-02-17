using System;
using System.Collections.Generic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.UI.Common.Panels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs
{
    [Serializable]
    [ConfigCategory(ConfigCategory.UI)]
    public class UIConfig : ScriptableConfig
    {
        [SerializeField, Required, AssetsOnly]
        private List<UIPanel> _prefabs = new List<UIPanel>();

        public List<UIPanel> Prefabs => _prefabs;
    }
}