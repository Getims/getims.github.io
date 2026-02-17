using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Project.Scripts.Infrastructure.Services.SaveLoad;
using UnityEngine;

namespace Project.Scripts.Infrastructure.Data
{
    public class GameDatabase : IDatabase
    {
        private readonly List<GameData> _allData;
        private readonly ISaveLoadService _saveLoadService;

        public GameDatabase()
        {
            _saveLoadService = new PlayerPrefsSaveLoadService();
            _allData = new List<GameData>();
            CreateData();
            ReloadData();
        }

        public void ReloadData()
        {
            int iterations = _allData.Count;

            for (int i = 0; i < iterations; i++)
            {
                GameData gameData = _allData[i];
                _saveLoadService.TryLoadData(ref gameData);
            }
        }

        public void SaveData()
        {
            int iterations = _allData.Count;

            for (int i = 0; i < iterations; i++)
            {
                GameData gameData = _allData[i];
                _saveLoadService.TrySaveData(gameData);
            }
        }

        public void DeleteData()
        {
            int iterations = _allData.Count;

            for (int i = 0; i < iterations; i++)
            {
                GameData gameData = _allData[i];
                _saveLoadService.TryDeleteData(ref gameData);
            }
        }

        public void DeleteData(GameData gameData)
        {
            _saveLoadService.TryDeleteData(ref gameData);
        }

        public IEnumerable<GameData> GetAllData() =>
            _allData;

        public T GetData<T>() where T : GameData
        {
            Type type = typeof(T);

            foreach (GameData data in _allData)
            {
                bool isMatches = data.GetType() == type;

                if (isMatches)
                    return (T)data;
            }

            LogDataNotFound(type);
            return null;
        }

        private void CreateData()
        {
            IEnumerable<Type> allData = GetInheritedClasses(typeof(GameData));

            foreach (Type type in allData)
            {
                var data = Activator.CreateInstance(type) as GameData;
                _allData.Add(data);
            }
        }

        private static IEnumerable<Type> GetInheritedClasses(Type targetType)
        {
            return Assembly
                .GetAssembly(targetType)
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(targetType));
        }

        private void LogDataNotFound(Type dataType)
        {
            string errorLog = $"Data of type <gb>({dataType}</gb> <rb>not found</rb>!";
            Debug.LogError(errorLog);
        }
    }
}