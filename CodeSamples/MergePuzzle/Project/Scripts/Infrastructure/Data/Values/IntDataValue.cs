using System;

namespace Project.Scripts.Infrastructure.Data.Values
{
    public class IntDataValue<TData> : DataValue<TData, int> where TData : GameData
    {
        public IntDataValue(TData data, Func<TData, int> getter, Action<TData, int> setter, Action<bool> tryToSave,
            bool enableValidation = true, Action<int> onValueChanged = null) : base(data, getter, setter, tryToSave,
            enableValidation, onValueChanged)
        {
        }

        public void Add(int value, bool autoSave = true)
        {
            if (value < 0)
                return;

            var newValue = _getter(_data) + value;
            Set(newValue, autoSave);
        }

        public void Spend(int value, bool autoSave = true)
        {
            if (value < 0 || value > Value)
                return;

            var newValue = _getter(_data) - value;
            Set(newValue, autoSave);
        }
    }
}