using Project.Scripts.Modules.DebugModule.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Modules.DebugModule.UI.Elements
{
    public class ButtonCheatElement : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private TMP_Text _title;

        private ICheat _cheat;

        public void Initialize(ICheat cheat)
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
            _cheat.Execute();
        }
    }
}