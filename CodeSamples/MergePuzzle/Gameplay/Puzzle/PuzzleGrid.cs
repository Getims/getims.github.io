using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Gameplay.Levels.Elements;
using Project.Scripts.Gameplay.Puzzle.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Puzzle
{
    public enum NeighborDirection
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class PuzzleGrid
    {
        private readonly Dictionary<Vector2Int, int> _snapPointIndices = new Dictionary<Vector2Int, int>();
        private readonly Dictionary<int, Vector3> _snapPointPositions = new Dictionary<int, Vector3>();

        private readonly Dictionary<Vector2Int, LevelElement>
            _gridElements = new Dictionary<Vector2Int, LevelElement>();

        private readonly float _snapThreshold;
        private readonly int _gridWidth;
        private readonly int _gridHeight;

        public PuzzleGrid(RectTransform container, int gridWidth, int gridHeight, float snapThreshold = 100f)
        {
            _snapThreshold = snapThreshold;
            _gridWidth = gridWidth;
            _gridHeight = gridHeight;
            GenerateGridPoints(container, gridWidth, gridHeight);
        }

        public bool TryMoveElements(IReadOnlyCollection<LevelElement> groupElements,
            IReadOnlyDictionary<LevelElement, Vector3> initialGroupPositions)
        {
            if (!TryGetTargetPositions(groupElements, out var groupTargetPositions))
            {
                return false;
            }

            if (!TryCreateMovementPlan(groupElements, initialGroupPositions, groupTargetPositions, out var movementPlan,
                    out var displacedElements))
            {
                return false;
            }

            ExecuteMovement(movementPlan, groupElements, displacedElements, initialGroupPositions);
            return true;
        }

        public Vector3 GetCorrectPositionFor(PuzzlePieceData pieceData)
        {
            var index = pieceData.Position.y * _gridWidth + pieceData.Position.x;
            if (_snapPointPositions.TryGetValue(index, out var position))
                return position;

            return Vector3.zero;
        }

        private (Vector3 point, float sqrDistance) FindNearestSnapPointInfo(Vector3 position)
        {
            if (_snapPointPositions.Count == 0) return (Vector3.zero, float.MaxValue);

            float minDistanceSqr = float.MaxValue;
            Vector3 closestPoint = Vector3.zero;

            foreach (var point in _snapPointPositions.Values)
            {
                float distanceSqr = Vector3.SqrMagnitude(position - point);
                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    closestPoint = point;
                }
            }

            return (closestPoint, minDistanceSqr);
        }

        public bool TryGetNearestSnapPoint(Vector3 position, out Vector3 snapPoint)
        {
            var (point, sqrDistance) = FindNearestSnapPointInfo(position);
            if (sqrDistance <= _snapThreshold * _snapThreshold)
            {
                snapPoint = point;
                return true;
            }

            snapPoint = Vector3.zero;
            return false;
        }

        private Vector3 GetNearestSnapPoint(Vector3 position)
        {
            return FindNearestSnapPointInfo(position).point;
        }

        public LevelElement GetNeighbor(Vector3 currentPosition, NeighborDirection direction)
        {
            if (!TryGetNeighborPosition(currentPosition, direction, out var neighborPosition))
                return null;

            return GetElementAt(neighborPosition);
        }

        public void RegisterElement(Vector3 position, LevelElement element)
        {
            var snapPoint = GetNearestSnapPoint(position);
            _gridElements[ToVector2Int(snapPoint)] = element;
        }

        public void UnregisterElement(Vector3 position)
        {
            var snapPoint = GetNearestSnapPoint(position);
            _gridElements.Remove(ToVector2Int(snapPoint));
        }

        public LevelElement GetElementAt(Vector3 position)
        {
            var snapPoint = GetNearestSnapPoint(position);
            _gridElements.TryGetValue(ToVector2Int(snapPoint), out var element);
            return element;
        }

        private bool TryGetTargetPositions(IReadOnlyCollection<LevelElement> groupElements,
            out Dictionary<LevelElement, Vector3> targetPositions)
        {
            targetPositions = new Dictionary<LevelElement, Vector3>();
            foreach (var element in groupElements)
            {
                if (TryGetNearestSnapPoint(element.transform.localPosition, out var snapPoint))
                    targetPositions[element] = snapPoint;
                else
                    return false;
            }

            return targetPositions.Values.Distinct().Count() == groupElements.Count;
        }

        private bool TryCreateMovementPlan(
            IReadOnlyCollection<LevelElement> currentGroupElements,
            IReadOnlyDictionary<LevelElement, Vector3> initialGroupPositions,
            IReadOnlyDictionary<LevelElement, Vector3> groupTargetPositions,
            out Dictionary<LevelElement, Vector3> movementPlan,
            out HashSet<LevelElement> displacedElements)
        {
            movementPlan = new Dictionary<LevelElement, Vector3>();

            displacedElements = new HashSet<LevelElement>();
            foreach (var targetPos in groupTargetPositions.Values)
            {
                var element = GetElementAt(targetPos);
                if (element != null && !currentGroupElements.Contains(element))
                {
                    displacedElements.Add(element);
                }
            }

            var trulyVacatedSlots = new Queue<Vector3>();
            var targetPosSet = new HashSet<Vector3>(groupTargetPositions.Values);
            foreach (var initialPos in initialGroupPositions.Values)
            {
                if (!targetPosSet.Contains(initialPos))
                {
                    trulyVacatedSlots.Enqueue(initialPos);
                }
            }

            if (displacedElements.Count > trulyVacatedSlots.Count)
            {
                return false;
            }

            foreach (var element in currentGroupElements)
            {
                movementPlan[element] = groupTargetPositions[element];
            }

            foreach (var element in displacedElements)
            {
                movementPlan[element] = trulyVacatedSlots.Dequeue();
            }

            return true;
        }

        private void ExecuteMovement(IReadOnlyDictionary<LevelElement, Vector3> movementPlan,
            IReadOnlyCollection<LevelElement> draggedGroup,
            IReadOnlyCollection<LevelElement> displacedElements,
            IReadOnlyDictionary<LevelElement, Vector3> initialGroupPositions)
        {
            foreach (var element in displacedElements)
            {
                UnregisterElement(element.transform.localPosition);
                element.transform.SetAsLastSibling();
            }

            foreach (var element in draggedGroup)
            {
                if (initialGroupPositions.TryGetValue(element, out var initialPos))
                    UnregisterElement(initialPos);

                element.transform.SetAsLastSibling();
            }

            foreach (var (element, newPos) in movementPlan)
            {
                element.SetPosition(newPos, true);
                RegisterElement(newPos, element);
            }
        }

        private void GenerateGridPoints(RectTransform container, int gridWidth, int gridHeight)
        {
            _snapPointIndices.Clear();
            _snapPointPositions.Clear();

            var containerSize = container.rect.size;
            var pieceDisplaySize = new Vector2(containerSize.x / gridWidth, containerSize.y / gridHeight);

            var startX = -containerSize.x * 0.5f + pieceDisplaySize.x * 0.5f;
            var startY = -containerSize.y * 0.5f + pieceDisplaySize.y * 0.5f;

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    var xPos = startX + (x * pieceDisplaySize.x);
                    var yPos = startY + (y * pieceDisplaySize.y);
                    var point = new Vector3(xPos, yPos, 0);
                    var index = y * gridWidth + x;

                    _snapPointIndices[ToVector2Int(point)] = index;
                    _snapPointPositions[index] = point;
                }
            }
        }

        private bool TryGetNeighborPosition(Vector3 currentPosition, NeighborDirection direction,
            out Vector3 neighborPosition)
        {
            neighborPosition = Vector3.zero;
            if (!_snapPointIndices.TryGetValue(ToVector2Int(currentPosition), out var index)) return false;

            int x = index % _gridWidth;
            int y = index / _gridWidth;

            switch (direction)
            {
                case NeighborDirection.Top: y++; break;
                case NeighborDirection.Bottom: y--; break;
                case NeighborDirection.Left: x--; break;
                case NeighborDirection.Right: x++; break;
            }

            if (x < 0 || x >= _gridWidth || y < 0 || y >= _gridHeight) return false;

            int neighborIndex = y * _gridWidth + x;
            if (!_snapPointPositions.TryGetValue(neighborIndex, out neighborPosition))
            {
                return false;
            }

            return true;
        }

        private Vector2Int ToVector2Int(Vector3 vector)
        {
            return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
        }
    }
}