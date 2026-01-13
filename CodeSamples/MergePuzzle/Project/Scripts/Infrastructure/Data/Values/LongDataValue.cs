using System;

namespace Project.Scripts.Infrastructure.Data.Values
{
    public class LongDataValue<TData> : DataValue<TData, long> where TData : GameData
    {
        public LongDataValue(TData data, Func<TData, long> getter, Action<TData, long> setter, Action<bool> tryToSave,
            bool enableValidation = true, Action<long> onValueChanged = null) : base(data, getter, setter, tryToSave,
            enableValidation, onValueChanged)
        {
        }

        public void Add(long value, bool autoSave = true, bool notify = true)
        {
            if (value < 0)
                return;

            var newValue = _getter(_data) + value;
            Set(newValue, autoSave, notify);
        }

        public void Spend(long value, bool autoSave = true, bool notify = true)
        {
            if (value < 0 || value > Value)
                return;

            var newValue = _getter(_data) - value;
            Set(newValue, autoSave, notify);
        }
    }
}