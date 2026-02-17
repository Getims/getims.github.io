using System.Collections.Generic;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Gameplay.Puzzle;
using Project.Scripts.Gameplay.Puzzle.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Levels.Elements
{
    public class ElementsFactory
    {
        private Vector2 _originalContainerSizeDelta;
        private bool _isOriginalSizeSaved;
        private ElementsPool _elementsPool;
        private RectTransform _container;

        public void Initialize(ElementsPool elementsPool)
        {
            _elementsPool = elementsPool;
            _container = elementsPool.Container;
        }

        public List<LevelElement> CreateElements(List<PuzzlePieceData> pieces, LevelConfig levelConfig,
            out PuzzleGrid puzzleGrid)
        {
            var createdElements = new List<LevelElement>();

            RestoreInitialContainerSize();
            AdjustContainerSize(levelConfig);

            puzzleGrid = new PuzzleGrid(_container, levelConfig.LevelWidth, levelConfig.LevelHeight);

            var containerSize = _container.rect.size;
            var pieceDisplayWidth = containerSize.x / levelConfig.LevelWidth;
            var pieceDisplayHeight = containerSize.y / levelConfig.LevelHeight;
            var pieceDisplaySize = new Vector2(pieceDisplayWidth, pieceDisplayHeight);

            foreach (var pieceData in pieces)
            {
                var element = CreateElement(pieceData, puzzleGrid, pieceDisplaySize);
                createdElements.Add(element);
            }

            return createdElements;
        }

        private void RestoreInitialContainerSize()
        {
            if (_isOriginalSizeSaved)
                _container.sizeDelta = _originalContainerSizeDelta;
        }

        private void AdjustContainerSize(LevelConfig levelConfig)
        {
            if (!_isOriginalSizeSaved)
            {
                _originalContainerSizeDelta = _container.sizeDelta;
                _isOriginalSizeSaved = true;
            }

            var sprite = levelConfig.Image;
            if (sprite == null || sprite.texture == null) return;

            var containerRect = _container.rect;
            var containerWidth = containerRect.width;
            var containerHeight = containerRect.height;

            var imageWidth = (float)sprite.texture.width;
            var imageHeight = (float)sprite.texture.height;
            var imageAspect = imageWidth / imageHeight;
            var containerAspect = containerWidth / containerHeight;

            float fittedWidth, fittedHeight;
            if (imageAspect > containerAspect)
            {
                fittedWidth = containerWidth;
                fittedHeight = fittedWidth / imageAspect;
            }
            else
            {
                fittedHeight = containerHeight;
                fittedWidth = fittedHeight * imageAspect;
            }

            _container.sizeDelta = new Vector2(fittedWidth - containerWidth + _container.sizeDelta.x,
                fittedHeight - containerHeight + _container.sizeDelta.y);
        }

        private LevelElement CreateElement(PuzzlePieceData pieceData, PuzzleGrid puzzleGrid, Vector2 pieceDisplaySize)
        {
            var position = puzzleGrid.GetCorrectPositionFor(pieceData);
            var element = _elementsPool.GetElement(position);

            var rectTransform = element.GetComponent<RectTransform>();
            rectTransform.sizeDelta = pieceDisplaySize;

            return element;
        }
    }
}