using Project.Scripts.Configs.Levels;
using Project.Scripts.UI.Common.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.MainMenu.Collections
{
    public class CollectionPreview : PopupPanel
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TMP_Text _name;

        [SerializeField]
        private TMP_Text _info;

        public void SetConfig(CollectionConfig collectionConfig)
        {
            _icon.sprite = collectionConfig.Sprite;
            _name.text = collectionConfig.Name;
            _info.text = collectionConfig.Info;
        }
    }
}