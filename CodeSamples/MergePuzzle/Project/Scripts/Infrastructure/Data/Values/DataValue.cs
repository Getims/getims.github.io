using System;

namespace Project.Scripts.Infrastructure.Data.Values
{
    public class DataValue<TData, TValue>
        where TData : GameData
    {
        protected readonly TData _data;
        protected readonly Func<TData, TValue> _getter;
        protected readonly Action<TData, TValue> _setter;
        protected readonly Action<bool> _tryToSave;
        protected bool _enableValidation;
        protected Action<TValue> _onValueChanged;

        public DataValue(TData data,
            Func<TData, TValue> getter,
            Action<TData, TValue> setter,
            Action<bool> tryToSave,
            bool enableValidation = true,
            Action<TValue> onValueChanged = null)
        {
            _onValueChanged = onValueChanged;
            _enableValidation = enableValidation;
            _data = data;
            _getter = getter;
            _setter = setter;
            _tryToSave = tryToSave;
        }

        public TValue Value => _getter(_data);

        public void Set(TValue value, bool autoSave = true, bool notify = true)
        {
            if (_enableValidation && !IsValid(value))
                return;

            _setter(_data, value);
            _tryToSave(autoSave);

            if (notify)
                _onValueChanged?.Invoke(value);
        }

        public void Notify()
        {
            _onValueChanged?.Invoke(Value);
        }

        protected virtual bool IsValid(TValue value)
        {
            return Equals(value, Value) == false;
        }
    }
}