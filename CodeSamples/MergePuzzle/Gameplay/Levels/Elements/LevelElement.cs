using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Project.Scripts.Configs;
using Project.Scripts.Gameplay.GameFlow;
using Project.Scripts.Gameplay.Puzzle;
using Project.Scripts.Gameplay.Puzzle.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Gameplay.Levels.Elements
{
    public class LevelElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private ElementView _elementView;

        [SerializeField]
        private ElementDebugger _elementDebugger;

        [SerializeField]
        private HintView _view;

        private PuzzleGrid _puzzleGrid;
        private PuzzleGroupManager _groupManager;
        private Action _onDropAction;
        private Action _onBeginDragAction;
        private Vector3 _dragStartOffset;
        private PuzzleGroup _currentGroup;
        private Dictionary<LevelElement, Vector3> _initialGroupLocalPositions;
        private PuzzleConfig _puzzleConfig;
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTW;
        private Tween _placeCall;

        public PuzzlePieceData PieceData { get; private set; }
        public ElementView View => _elementView;

        public void SetupConfig(PuzzleConfig puzzleConfig)
        {
            _puzzleConfig = puzzleConfig;
        }

        public void Initialize(PuzzlePieceData pieceData, PuzzleGrid puzzleGrid, PuzzleGroupManager groupManager,
            Action onDropAction, Action onBeginDragAction)
        {
            PieceData = pieceData;
            _puzzleGrid = puzzleGrid;
            _groupManager = groupManager;
            _onDropAction = onDropAction;
            _onBeginDragAction = onBeginDragAction;

            _elementView.Initialize(PieceData, puzzleGrid);
            _puzzleGrid.RegisterElement(transform.localPosition, this);

            _elementDebugger.Initialize(PieceData);
        }

        public void UpdateValidationState()
        {
            _elementView.UpdateFrameState();
        }

        public void SetHint(bool isHinting)
        {
            _view.SetHint(isHinting);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanDrag())
                return;

            StaticInputService.SetDragBlock(true);
            _onBeginDragAction?.Invoke();

            _currentGroup = _groupManager.GetGroupFor(this);
            if (_currentGroup == null)
                return;

            ScaleElementGroup();
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, eventData.position,
                eventData.pressEventCamera, out var localPoint);

            _dragStartOffset = transform.localPosition - (Vector3)localPoint;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_currentGroup == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, eventData.position,
                eventData.pressEventCamera, out var localPoint);
            Vector3 targetPos = (Vector3)localPoint + _dragStartOffset;
            Vector3 delta = targetPos - transform.localPosition;

            foreach (var element in _currentGroup.Elements)
            {
                element.transform.localPosition += delta;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_currentGroup == null) return;

            ResetElementGroupScale();

            if (_puzzleGrid.TryMoveElements(_currentGroup.Elements, _initialGroupLocalPositions))
            {
                _placeCall.Kill();
                _placeCall = DOVirtual
                    .DelayedCall(_puzzleConfig.ElementPlaceDuration + 0.02f, OnDropAnimationComplete)
                    .SetLink(gameObject);
            }
            else
            {
                RevertGroupToInitialPositions();
            }

            ResetDragState();
        }

        public void BounceScaleElementGroup()
        {
            _currentGroup = _groupManager.GetGroupFor(this);
            if (_currentGroup == null)
                return;

            StaticInputService.SetAnimationBlock(true);

            Sequence sequence = DOTween.Sequence();
            float animationStepDuration = _puzzleConfig.DragScaleTime * .5f;
            sequence.SetLink(gameObject);
            sequence.AppendCallback(() => ScaleElementGroup(true));
            sequence.AppendInterval(animationStepDuration);
            sequence.AppendCallback(() => ResetElementGroupScale(true));
            sequence.AppendInterval(animationStepDuration);
            sequence.AppendCallback(() =>
            {
                StaticInputService.SetAnimationBlock(false);
                _currentGroup = null;
            });
            sequence.Play();
        }

        public void SetPosition(Vector3 newPosition, bool animated = false)
        {
            if (animated)
            {
                _moveTW?.Kill();
                _moveTW = transform
                    .DOLocalMove(newPosition, _puzzleConfig.ElementPlaceDuration)
                    .SetEase(Ease.OutSine)
                    .OnComplete(() => StaticInputService.SetDragBlock(false));

                _moveTW.SetLink(gameObject);
            }
            else
            {
                _moveTW?.Complete();
                transform.localPosition = newPosition;
                StaticInputService.SetDragBlock(false);
            }
        }

        private void RevertGroupToInitialPositions()
        {
            if (_initialGroupLocalPositions == null)
                return;

            foreach (var (element, initialPos) in _initialGroupLocalPositions)
            {
                element.SetPosition(initialPos, true);
            }
        }

        private void ScaleElementGroup(bool animate = false)
        {
            _initialGroupLocalPositions = _currentGroup.Elements.ToDictionary(e => e, e => e.transform.localPosition);

            Vector3 groupCenter = _initialGroupLocalPositions.Values.Aggregate(Vector3.zero, (s, v) => s + v) /
                                  _initialGroupLocalPositions.Count;

            float time = animate ? _puzzleConfig.DragScaleTime * .5f : 0;

            foreach (var element in _currentGroup.Elements)
            {
                element.transform.SetAsLastSibling();

                Vector3 initialPos = _initialGroupLocalPositions[element];
                Vector3 dirFromCenter = initialPos - groupCenter;
                Vector3 offset = dirFromCenter * (_puzzleConfig.DragScaleMultiplier - 1f);

                element.View.SetScale(_puzzleConfig.DragScaleMultiplier, offset, time);
            }
        }

        private void ResetElementGroupScale(bool animate = false)
        {
            float time = animate ? _puzzleConfig.DragScaleTime * .5f : 0;

            foreach (var element in _currentGroup.Elements)
            {
                element.View.ResetScale(time);
            }
        }

        private bool CanDrag()
        {
            return StaticInputService.CanDrag;
        }

        private void ResetDragState()
        {
            _currentGroup = null;
            _initialGroupLocalPositions = null;
        }

        private void OnDropAnimationComplete()
        {
            _onDropAction?.Invoke();
        }
    }
}