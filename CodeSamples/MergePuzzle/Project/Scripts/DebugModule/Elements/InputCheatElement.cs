using Project.Scripts.DebugModule.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.DebugModule.Elements
{
    public class InputCheatElement : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _input;

        [SerializeField]
        private Button _button;

        [SerializeField]
        private TMP_Text _title;

        private ICheat<int> _cheat;

        public void Initialize(ICheat<int> cheat)
        {
            _cheat = cheat;
            _title.text = _cheat.Name;
        }

        private void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            int value = int.Parse(_input.text);
            _cheat.Execute(value);
        }
    }
}