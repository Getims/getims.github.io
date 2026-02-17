using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Gameplay.Factory.Utils
{
    public static class SpawnZoneHelper
    {
        public static List<Vector3> DistributeSquares(Vector3 center, Vector3 size, int count)
        {
            List<Vector3> positions = new List<Vector3>();
            float rectWidth = size.x;
            float rectHeight = size.z;

            int cols = Mathf.CeilToInt(Mathf.Sqrt(count * rectWidth / rectHeight));
            int rows = Mathf.CeilToInt((float)count / cols);

            float stepX = rectWidth / cols;
            float stepZ = rectHeight / rows;

            float startX = center.x - rectWidth / 2 + stepX / 2;
            float startZ = center.z - rectHeight / 2 + stepZ / 2;

            int placed = 0;
            for (int i = 0; i < rows && placed < count; i++)
            {
                for (int j = 0; j < cols && placed < count; j++)
                {
                    float x = startX + j * stepX;
                    float z = startZ + i * stepZ;
                    positions.Add(new Vector3(x, center.y, z));
                    placed++;
                }
            }

            return positions;
        }
    }
}