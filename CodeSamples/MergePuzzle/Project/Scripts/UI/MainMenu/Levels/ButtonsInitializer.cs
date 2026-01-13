using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.Scripts.UI.MainMenu.Levels
{
    public class ButtonsInitializer : MonoBehaviour
    {
        [SerializeField]
        private List<LevelButton> _buttons;

        [SerializeField]
        private GridLayoutGroup _gridLayoutGroup;

        [SerializeField, HideInInspector]
        private int _levelsPerCollection = 1;

        [Inject] private ILevelsDataService _levelsDataService;
        private LevelsConfigProvider _levelsConfigProvider;
        private readonly List<Sprite> _slicedSprites = new List<Sprite>();
        private int _cachedCollectionIndex = -1;

        public void Initialize(LevelsConfigProvider levelsConfigProvider, int currentLevelIndex)
        {
            _levelsConfigProvider = levelsConfigProvider;
            SliceCollectionSprite(currentLevelIndex);
        }

        public void UpdateButtonsState(int currentLevelIndex)
        {
            SliceCollectionSprite(currentLevelIndex);

            int collectionIndex = currentLevelIndex / _levelsPerCollection;
            int min = collectionIndex * _levelsPerCollection;

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (i >= _slicedSprites.Count)
                {
                    Debug.LogWarning($"Not enough sliced sprites for button index {i}. Slicing might have failed.");
                    continue;
                }

                var button = _buttons[i];
                button.Initialize(min + i);
                button.SetImage(_slicedSprites[i]);
                button.UpdateState(currentLevelIndex);
            }
        }

        private void SliceCollectionSprite(int currentLevelIndex)
        {
            int collectionIndex = currentLevelIndex / _levelsPerCollection;
            if (collectionIndex == _cachedCollectionIndex && _slicedSprites.Count > 0)
                return;

            if (collectionIndex > _cachedCollectionIndex && _cachedCollectionIndex != -1)
                SendCollectionUnlockedEvent(collectionIndex);

            _cachedCollectionIndex = collectionIndex;
            _slicedSprites.Clear();

            var collectionConfig = _levelsConfigProvider.GetCollectionConfig(collectionIndex);
            var sourceSprite = collectionConfig.Sprite;
            var texture = sourceSprite.texture;
            var rect = sourceSprite.rect;

            int cols = _gridLayoutGroup.constraintCount;
            int rows = Mathf.CeilToInt((float)_buttons.Count / cols);

            if (rows == 0 || cols == 0)
            {
                Debug.LogError("GridLayoutGroup is not configured correctly (rows or columns is zero).");
                return;
            }

            int sliceWidth = (int)(rect.width / cols);
            int sliceHeight = (int)(rect.height / rows);

            for (int i = 0; i < _buttons.Count; i++)
            {
                int row = i / cols;
                int col = i % cols;

                int x = (int)rect.x + col * sliceWidth;
                int y = (int)rect.y + (rows - row - 1) * sliceHeight; // учёт координат сверху вниз

                Rect subRect = new Rect(x, y, sliceWidth, sliceHeight);
                var slice = Sprite.Create(texture, subRect, new Vector2(0.5f, 0.5f), sourceSprite.pixelsPerUnit);
                _slicedSprites.Add(slice);
            }
        }

        private void SendCollectionUnlockedEvent(int collectionIndex)
        {
            _levelsDataService.CollectionsUnlocked.Set(collectionIndex);
        }

        [Button]
        private void CollectButtons()
        {
            _buttons = transform.GetComponentsInChildren<LevelButton>(false).ToList();
            _levelsPerCollection = _buttons.Count;

            if (_gridLayoutGroup != null)
            {
                int cols = _gridLayoutGroup.constraintCount;
                if (cols > 0)
                {
                    int rows = Mathf.CeilToInt((float)_buttons.Count / cols);
                    int gridCapacity = cols * rows;
                    if (_levelsPerCollection > gridCapacity)
                    {
                        Debug.LogWarning(
                            $"Number of buttons ({_levelsPerCollection}) exceeds grid capacity ({gridCapacity}).");
                    }
                }
            }
            else
            {
                Debug.LogError("GridLayoutGroup component not found on this GameObject. Please add one.");
            }
        }
    }
}