using System;
using Project.Scripts.Configs.Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.MainMenu.Collections
{
    public class CollectionButton : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TMP_Text _name;

        [SerializeField]
        private GameObject _lockedContainer;

        [SerializeField]
        private Button _claimButton;

        private CollectionConfig _collectionConfig;
        private bool _isEnabled;

        public event Action<CollectionConfig> OnClick;

        public CollectionConfig CollectionConfig => _collectionConfig;

        public void SetConfig(CollectionConfig collectionConfig)
        {
            _collectionConfig = collectionConfig;
            _icon.sprite = collectionConfig.Sprite;
            _name.text = collectionConfig.Name;
        }

        public void SetState(bool isEnabled)
        {
            _lockedContainer.SetActive(!isEnabled);
            _isEnabled = isEnabled;
        }

        private void Start()
        {
            if (_claimButton != null)
                _claimButton.onClick.AddListener(OnClaimButtonClick);
        }

        private void OnDestroy()
        {
            if (_claimButton != null)
                _claimButton.onClick.RemoveListener(OnClaimButtonClick);
        }

        private void OnClaimButtonClick()
        {
            if (!_isEnabled)
                return;
            OnClick?.Invoke(_collectionConfig);
        }
    }
}