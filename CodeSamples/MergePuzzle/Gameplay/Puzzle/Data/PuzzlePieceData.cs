using System;
using UnityEngine;

namespace Project.Scripts.Gameplay.Puzzle.Data
{
    [Serializable]
    public class PuzzlePieceData
    {
        public int Id { get; }
        public Vector2Int Position { get; }
        public Sprite PieceSprite { get; }

        public int TopNeighborId { get; }
        public int BottomNeighborId { get; }
        public int LeftNeighborId { get; }
        public int RightNeighborId { get; }

        public PuzzlePieceData(int id, Vector2Int position, Sprite pieceSprite,
            int topNeighborId = -1, int bottomNeighborId = -1, int leftNeighborId = -1, int rightNeighborId = -1)
        {
            Id = id;
            Position = position;
            PieceSprite = pieceSprite;
            TopNeighborId = topNeighborId;
            BottomNeighborId = bottomNeighborId;
            LeftNeighborId = leftNeighborId;
            RightNeighborId = rightNeighborId;
        }
    }
}