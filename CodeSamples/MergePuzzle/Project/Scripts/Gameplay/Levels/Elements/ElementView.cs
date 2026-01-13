using System;
using System.Collections.Generic;
using DG.Tweening;
using Project.Scripts.Gameplay.Puzzle;
using Project.Scripts.Gameplay.Puzzle.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Levels.Elements
{
    public class ElementView : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private Transform _scaleContainer;

        [SerializeField]
        private List<DirectionalFrame> _incorrectFrames;

        private readonly Dictionary<NeighborDirection, Image> _framesMap =
            new Dictionary<NeighborDirection, Image>();

        private PuzzlePieceData _pieceData;
        private PuzzleGrid _puzzleGrid;
        private Tweener _scaleTW;
        private Tweener _moveTW;

        [Serializable]
        private struct DirectionalFrame
        {
            public NeighborDirection Direction;
            public Image FrameImage;
        }

        public void Initialize(PuzzlePieceData pieceData, PuzzleGrid puzzleGrid)
        {
            _puzzleGrid = puzzleGrid;
            _pieceData = pieceData;
            _image.sprite = _pieceData.PieceSprite;
        }

        private void Awake()
        {
            foreach (var frame in _incorrectFrames)
                _framesMap[frame.Direction] = frame.FrameImage;
        }

        public void UpdateFrameState()
        {
            UpdateFrameState(NeighborDirection.Top, _pieceData.TopNeighborId);
            UpdateFrameState(NeighborDirection.Bottom, _pieceData.BottomNeighborId);
            UpdateFrameState(NeighborDirection.Left, _pieceData.LeftNeighborId);
            UpdateFrameState(NeighborDirection.Right, _pieceData.RightNeighborId);
        }

        private void UpdateFrameState(NeighborDirection direction, int expectedNeighborId)
        {
            if (_framesMap.TryGetValue(direction, out var image))
            {
                var isCorrect = CheckNeighbor(direction, expectedNeighborId);
                image.enabled = expectedNeighborId == -1 || !isCorrect;
            }
        }

        private bool CheckNeighbor(NeighborDirection direction, int expectedNeighborId)
        {
            var neighbor = _puzzleGrid.GetNeighbor(transform.localPosition, direction);
            return expectedNeighborId != -1
                ? neighbor != null && neighbor.PieceData.Id == expectedNeighborId
                : neighbor == null;
        }

        public void SetScale(float scale, Vector3 offset, float time = 0)
        {
            if (time > 0)
            {
                _scaleTW?.Kill();
                _moveTW?.Kill();

                _scaleTW = _scaleContainer.DOScale(scale, time)
                    .SetEase(Ease.InSine).SetLink(gameObject);

                _moveTW = _scaleContainer.DOLocalMove(offset, time)
                    .SetEase(Ease.InSine).SetLink(gameObject);
                return;
            }

            _scaleContainer.localScale = Vector3.one * scale;
            _scaleContainer.localPosition = offset;
        }

        public void ResetScale(float time = 0)
        {
            if (time > 0)
            {
                _scaleTW?.Kill();
                _moveTW?.Kill();

                _scaleTW = _scaleContainer.DOScale(1, time)
                    .SetEase(Ease.OutSine).SetLink(gameObject);

                _moveTW = _scaleContainer.DOLocalMove(Vector3.zero, time)
                    .SetEase(Ease.OutSine).SetLink(gameObject);
                return;
            }

            _scaleContainer.localScale = Vector3.one;
            _scaleContainer.localPosition = Vector3.zero;
        }
    }
}