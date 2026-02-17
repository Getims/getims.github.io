using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Gameplay.Levels.Elements;
using Project.Scripts.Gameplay.Puzzle;
using Project.Scripts.Gameplay.Puzzle.Data;
using Utils = Project.Scripts.Infrastructure.Utilities.Utils;

namespace Project.Scripts.Gameplay.Levels
{
    public class HintSearcher
    {
        private readonly List<LevelElement> _createdElements;
        private readonly PuzzleGroupManager _groupManager;
        private readonly PuzzleGrid _puzzleGrid;

        public HintSearcher(List<LevelElement> elements, PuzzleGroupManager groupManager, PuzzleGrid puzzleGrid)
        {
            _createdElements = elements;
            _groupManager = groupManager;
            _puzzleGrid = puzzleGrid;
        }

        public bool TryFindHint(out List<LevelElement> hintElements)
        {
            hintElements = new List<LevelElement>();
            var singleElements = _createdElements.Where(e => _groupManager.GetGroupFor(e).Elements.Count == 1).ToList();
            if (singleElements.Count == 0)
                return false;

            Utils.Shuffle(singleElements);
            foreach (var currentElement in singleElements)
            {
                foreach (var targetElement in _createdElements)
                {
                    if (currentElement == targetElement) continue;

                    if (IsConnectedToAnyNeighbor(currentElement.PieceData, targetElement))
                    {
                        hintElements.Add(currentElement);
                        hintElements.Add(targetElement);
                        return true;
                    }
                }
            }

            if (hintElements.Count <= 1)
            {
                hintElements.Clear();
            }

            return hintElements.Count > 0;
        }

        private bool IsConnectedToAnyNeighbor(PuzzlePieceData sourcePieceData, LevelElement targetElement)
        {
            foreach (NeighborDirection direction in Enum.GetValues(typeof(NeighborDirection)))
            {
                var neighbor = _puzzleGrid.GetNeighbor(targetElement.transform.localPosition, direction);
                if (neighbor == null)
                    continue;
                if (IsConnected(sourcePieceData, neighbor.PieceData, direction))
                    return true;
            }

            return false;
        }

        private bool IsConnected(PuzzlePieceData piece, PuzzlePieceData neighbor, NeighborDirection direction)
        {
            var expectedId = direction switch
            {
                NeighborDirection.Top => piece.TopNeighborId,
                NeighborDirection.Bottom => piece.BottomNeighborId,
                NeighborDirection.Left => piece.LeftNeighborId,
                NeighborDirection.Right => piece.RightNeighborId,
                _ => -1
            };
            return neighbor.Id == expectedId;
        }
    }
}