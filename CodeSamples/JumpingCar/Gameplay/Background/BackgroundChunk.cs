using UnityEngine;

namespace Project.Scripts.Gameplay.Background
{
    public class BackgroundChunk : MonoBehaviour
    {
        [SerializeField]
        private float _height = 20;

        public Transform Transform => transform;
        public float Height => _height;
    }
}