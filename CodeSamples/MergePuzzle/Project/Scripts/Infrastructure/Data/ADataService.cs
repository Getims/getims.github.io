using System;
using Project.Scripts.Infrastructure.Data.Values;

namespace Project.Scripts.Infrastructure.Data
{
    public abstract class ADataService
    {
        private readonly IDatabase _database;

        protected ADataService(IDatabase database) =>
            _database = database;

        protected void TryToSave(bool autoSave)
        {
            if (!autoSave)
                return;

            _database.SaveData();
        }
    }

    public abstract class ADataService<TData> : ADataService where TData : GameData
    {
        protected readonly TData _data;

        protected ADataService(IDatabase database) : base(database)
        {
            _data = database.GetData<TData>();
        }

        protected DataValue<TData, TValue> CreateValue<TValue>(
            Func<TData, TValue> getter,
            Action<TData, TValue> setter,
            Action<TValue> onValueChanged = null,
            bool enableValidation = true)
        {
            return new DataValue<TData, TValue>(
                _data,
                getter,
                setter,
                TryToSave,
                enableValidation,
                onValueChanged
            );
        }

        protected LongDataValue<TData> CreateLongValue(
            Func<TData, long> getter,
            Action<TData, long> setter,
            Action<long> onValueChanged = null,
            bool enableValidation = true)
        {
            return new LongDataValue<TData>(_data, getter, setter, TryToSave, enableValidation, onValueChanged);
        }

        protected IntDataValue<TData> CreateIntValue(
            Func<TData, int> getter,
            Action<TData, int> setter,
            Action<int> onValueChanged = null,
            bool enableValidation = true)
        {
            return new IntDataValue<TData>(_data, getter, setter, TryToSave, enableValidation, onValueChanged);
        }
    }
}