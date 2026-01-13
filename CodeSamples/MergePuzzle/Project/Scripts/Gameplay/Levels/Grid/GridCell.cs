using UnityEngine;

namespace Project.Scripts.Gameplay.Levels.Grid
{
    public class GridCell : MonoBehaviour
    {
        public void SetPosition(Vector3 newPosition)
        {
            transform.localPosition = newPosition;
        }
    }
}