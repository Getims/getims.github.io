using Project.Scripts.DebugModule.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.DebugModule.Elements
{
    public class SliderCheatElement : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        [SerializeField]
        private TMP_Text _title;

        private ICheat<float> _cheat;

        public void Initialize(ICheat<float> cheat, float startValue = 1)
        {
            _cheat = cheat;
            _title.text = $"{_cheat.Name}: {startValue}";
            _slider.SetValueWithoutNotify(startValue);
        }

        private void Start()
        {
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            _title.text = $"{_cheat.Name}: {value:F2}";
            _cheat.Execute(value);
        }
    }
}