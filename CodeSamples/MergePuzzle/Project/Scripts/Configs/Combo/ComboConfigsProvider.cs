using System.Collections.Generic;
using Project.Scripts.Core.Constants;
using Project.Scripts.Infrastructure.Configs;
using UnityEngine;

namespace Project.Scripts.Configs.Combo
{
    [ConfigCategory(ConfigCategory.Game)]
    public class ComboConfigsProvider : ScriptableConfig
    {
        [SerializeField]
        private List<ComboConfig> _comboConfigs;

        public ComboConfig GetComboConfig(int comboLevel)
        {
            comboLevel = Mathf.Clamp(comboLevel, 0, _comboConfigs.Count - 1);
            return _comboConfigs[comboLevel];
        }
    }
}