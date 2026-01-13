using System;
using System.Collections.Generic;
using Project.Scripts.Core.Constants;
using UnityEngine;

namespace Project.Scripts.Infrastructure.Configs
{
    public interface IConfigsProvider
    {
        T GetConfig<T>() where T : ScriptableConfig;
    }

    public class ConfigsProvider : IConfigsProvider
    {
        private readonly List<ScriptableConfig> _allConfigs;

        public ConfigsProvider()
        {
            _allConfigs = new List<ScriptableConfig>();
            FindAllConfigs();
        }

        public T GetConfig<T>() where T : ScriptableConfig
        {
            Type type = typeof(T);
            foreach (ScriptableConfig ScriptableConfig in _allConfigs)
            {
                bool isTypeMatches = ScriptableConfig.GetType() == type;

                if (!isTypeMatches)
                    continue;

                if (!IsValidConfig())
                    continue;

                return ScriptableConfig as T;
            }

            LogConfigNotFound(type);
            return null;
        }

        protected bool IsValidConfig() => true;

        private void FindAllConfigs()
        {
            ScriptableConfig[] allConfigsMeta = Resources.LoadAll<ScriptableConfig>(path: GameConstants.GLOBAL_PATH);

            foreach (ScriptableConfig ScriptableConfig in allConfigsMeta)
                _allConfigs.Add(ScriptableConfig);
        }

        private static void LogConfigNotFound(Type configType)
        {
            string errorLog = $"Config Meta of type ({configType} not found!";
            Debug.LogError(errorLog);
        }
    }
}