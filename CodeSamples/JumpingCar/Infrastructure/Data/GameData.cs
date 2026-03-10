using System;
using UnityEngine;

namespace Project.Scripts.Infrastructure.Data
{
    [Serializable]
    public abstract class GameData
    {
        public string DataKey => GetType().Name;

        public string GetDataPath() =>
            $"{Application.persistentDataPath}/{DataKey}.json";
    }
}