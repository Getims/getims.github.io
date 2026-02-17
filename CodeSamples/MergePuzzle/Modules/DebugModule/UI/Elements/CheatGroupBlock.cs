using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Modules.DebugModule.UI.Elements
{
    public class CheatGroupBlock : MonoBehaviour
    {
        [SerializeField]
        private Button _toggleButton;

        [SerializeField]
        private TMP_Text _groupTitle;

        [SerializeField]
        private Transform _cheatsContainer;

        private bool _isExpanded = true;
        private string _title;

        public void Initialize(string title)
        {
            _title = title;
            _toggleButton.onClick.AddListener(Toggle);
            Toggle();
        }

        public Transform GetCheatsContainer() => _cheatsContainer;

        private void Toggle()
        {
            _isExpanded = !_isExpanded;
            _cheatsContainer.gameObject.SetActive(_isExpanded);

            string icon = _isExpanded ? "<" : ">";
            _groupTitle.text = $"{_title} {icon}";
        }
    }
}