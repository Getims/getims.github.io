using Project.Scripts.Core.Constants;
using Project.Scripts.Infrastructure.Configs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Configs.Levels
{
    [ConfigCategory(ConfigCategory.Game)]
    public class LevelConfig : ScriptableConfig
    {
        [SerializeField]
        private Sprite _image;

        [SerializeField, MinValue(2)]
        private int _levelWidth = 2;

        [SerializeField, MinValue(2)]
        private int _levelHeight = 2;

        [SerializeField, Min(0)]
        private int _moneyReward = 0;

        public Sprite Image => _image;
        public int LevelWidth => _levelWidth;
        public int LevelHeight => _levelHeight;
        public int Reward => _moneyReward;
    }
}