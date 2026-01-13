using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Core.Utilities;
using Project.Scripts.Gameplay.Levels.Elements;

namespace Project.Scripts.Gameplay.Puzzle
{
    public class PuzzleGroup
    {
        public readonly HashSet<LevelElement> Elements = new HashSet<LevelElement>();
    }

    public class PuzzleGroupManager
    {
        private readonly PuzzleGrid _grid;
        private readonly List<LevelElement> _allElements;

        private readonly List<PuzzleGroup> _groups = new List<PuzzleGroup>();

        private readonly Dictionary<LevelElement, PuzzleGroup> _elementToGroupMap =
            new Dictionary<LevelElement, PuzzleGroup>();

        public PuzzleGroupManager(PuzzleGrid grid, List<LevelElement> allElements)
        {
            _grid = grid;
            _allElements = allElements;
        }

        public int GetTotalGroups()
        {
            return _groups.Count;
        }

        public PuzzleGroup GetGroupFor(LevelElement element)
        {
            return _elementToGroupMap.GetValueOrDefault(element);
        }

        public List<PuzzleGroup> RebuildGroups()
        {
            var oldElementToGroupMap = new Dictionary<LevelElement, PuzzleGroup>(_elementToGroupMap);
            var isFirstBuild = oldElementToGroupMap.Count == 0;

            _elementToGroupMap.Clear();
            _groups.Clear();

            var visitedElements = new HashSet<LevelElement>();

            foreach (var element in _allElements)
            {
                if (!visitedElements.Contains(element))
                {
                    var newGroup = BuildGroup(element, visitedElements);
                    _groups.Add(newGroup);
                }
            }

            if (isFirstBuild)
            {
                return new List<PuzzleGroup>();
            }

            var updatedGroups = new HashSet<PuzzleGroup>();
            foreach (var kvp in _elementToGroupMap)
            {
                var element = kvp.Key;
                var newGroup = kvp.Value;

                if (oldElementToGroupMap.TryGetValue(element, out var oldGroup))
                {
                    if (newGroup.Elements.Count > oldGroup.Elements.Count)
                    {
                        updatedGroups.Add(newGroup);
                    }
                }
            }

            //LogGroups();
            //LogDictionary();

            return updatedGroups.ToList();
        }

        private PuzzleGroup BuildGroup(LevelElement startElement, HashSet<LevelElement> visitedElements)
        {
            var newGroup = new PuzzleGroup();
            var queue = new Queue<LevelElement>();

            queue.Enqueue(startElement);
            visitedElements.Add(startElement);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                newGroup.Elements.Add(current);
                _elementToGroupMap[current] = newGroup; // Set group for current element

                foreach (NeighborDirection direction in Enum.GetValues(typeof(NeighborDirection)))
                {
                    var neighbor = _grid.GetNeighbor(current.transform.localPosition, direction);
                    if (neighbor != null && !visitedElements.Contains(neighbor) &&
                        AreElementsConnected(current, neighbor, direction))
                    {
                        visitedElements.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return newGroup;
        }

        private bool AreElementsConnected(LevelElement element, LevelElement neighbor, NeighborDirection direction)
        {
            var expectedId = direction switch
            {
                NeighborDirection.Top => element.PieceData.TopNeighborId,
                NeighborDirection.Bottom => element.PieceData.BottomNeighborId,
                NeighborDirection.Left => element.PieceData.LeftNeighborId,
                NeighborDirection.Right => element.PieceData.RightNeighborId,
                _ => -1
            };
            return neighbor.PieceData.Id == expectedId;
        }

        private void LogGroups()
        {
            string log = string.Empty;
            int id = 0;

            foreach (var group in _groups)
            {
                log += $"Group [{id}]:";
                foreach (var element in group.Elements)
                    log += $" {element.PieceData.Id},";

                log += "\n";
                id++;
            }

            Utils.Log("Groups", log);
        }

        private void LogDictionary()
        {
            string log = string.Empty;

            foreach (var elementToGroup in _elementToGroupMap)
            {
                log += $"Element [{elementToGroup.Key.PieceData.Id}] :";
                foreach (var element in elementToGroup.Value.Elements)
                {
                    log += $" {element.PieceData.Id},";
                }

                log += "\n";
            }

            Utils.Log("Dictionary", log);
        }
    }
}