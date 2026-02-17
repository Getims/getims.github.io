using System.Collections.Generic;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.Runtime.Constants;
using UnityEngine;

namespace Project.Scripts.Configs.Levels
{
    [ConfigCategory(ConfigCategory.Game)]
    public class LevelsConfigProvider : ScriptableConfig
    {
        [SerializeField]
        private List<LevelConfig> _levels = new List<LevelConfig>();

        [SerializeField]
        private List<CollectionConfig> _collection = new List<CollectionConfig>();

        public int LevelsCount => _levels.Count;
        public int CollectionsCount => _collection.Count;

        public LevelConfig GetLevel(int levelId)
        {
            int levelsCount = _levels.Count;
            while (levelId >= levelsCount)
                levelId -= levelsCount;

            if (levelId < 0)
                levelId = 0;

            return _levels[levelId];
        }

        public CollectionConfig GetCollectionConfig(int index)
        {
            if (index < 0 || index >= _collection.Count)
            {
                Debug.LogWarning($"Index {index} is out of range for Level Button Collections!");
                index = 0;
            }

            return _collection[index];
        }
    }
}