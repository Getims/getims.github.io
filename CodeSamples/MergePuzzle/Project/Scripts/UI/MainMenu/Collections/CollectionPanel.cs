using Project.Scripts.Configs.Levels;
using Project.Scripts.Data;
using Project.Scripts.Infrastructure.Configs;
using Project.Scripts.UI.Common.Panels;
using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.MainMenu.Collections
{
    public class CollectionPanel : PopupPanel
    {
        [SerializeField]
        private CollectionButton _collectionButtonPrefab;

        [SerializeField]
        private Transform _collectionButtonsContainer;

        [SerializeField]
        private CollectionPreview _collectionPreview;

        [Inject] private IConfigsProvider _configsProvider;
        [Inject] private ILevelsDataService _levelsDataService;

        public override void Initialize()
        {
            base.Initialize();
            HideEvent += DestroySelf;

            var levelsConfigProvider = _configsProvider.GetConfig<LevelsConfigProvider>();
            int collectionsCount = levelsConfigProvider.CollectionsCount;
            int collectionsUnlocked = _levelsDataService.CollectionsUnlocked.Value;

            _collectionButtonsContainer.gameObject.SetActive(false);

            for (int i = 0; i < collectionsCount; i++)
            {
                var collectionButton = Instantiate(_collectionButtonPrefab, _collectionButtonsContainer);
                var collectionConfig = levelsConfigProvider.GetCollectionConfig(i);

                collectionButton.SetConfig(collectionConfig);
                collectionButton.SetState(i < collectionsUnlocked);
                collectionButton.OnClick += ShowPreview;
            }

            _collectionButtonsContainer.gameObject.SetActive(true);
        }

        private void ShowPreview(CollectionConfig collectionConfig)
        {
            _collectionPreview.SetConfig(collectionConfig);
            _collectionPreview.Show();
        }
    }
}