using System.Collections.Generic;
using Project.Scripts.Configs.Levels;
using Project.Scripts.Gameplay.Puzzle.Data;
using UnityEngine;

namespace Project.Scripts.Gameplay.Puzzle
{
    public class PuzzleGenerator
    {
        public List<PuzzlePieceData> GeneratePieces(LevelConfig levelConfig)
        {
            var pieces = new List<PuzzlePieceData>();
            var image = levelConfig.Image;
            var texture = image.texture;
            var width = levelConfig.LevelWidth;
            var height = levelConfig.LevelHeight;

            var pieceWidth = texture.width / width;
            var pieceHeight = texture.height / height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var id = y * width + x;
                    var position = new Vector2Int(x, y);

                    var rect = new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight);
                    var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));

                    var topNeighborId = (y + 1 < height) ? (y + 1) * width + x : -1;
                    var bottomNeighborId = (y - 1 >= 0) ? (y - 1) * width + x : -1;
                    var leftNeighborId = (x - 1 >= 0) ? y * width + (x - 1) : -1;
                    var rightNeighborId = (x + 1 < width) ? y * width + (x + 1) : -1;

                    var pieceData = new PuzzlePieceData(id, position, sprite,
                        topNeighborId, bottomNeighborId, leftNeighborId, rightNeighborId);

                    pieces.Add(pieceData);
                }
            }

            return pieces;
        }
    }
}