using System.Collections.Generic;

namespace Project.Scripts.Infrastructure.Data
{
    public interface IDatabase
    {
        void ReloadData();
        void SaveData();
        void DeleteData();
        void DeleteData(GameData gameData);
        IEnumerable<GameData> GetAllData();
        T GetData<T>() where T : GameData;
    }
}