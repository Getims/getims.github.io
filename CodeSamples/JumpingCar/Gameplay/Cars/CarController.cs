using Project.Scripts.Configs.Cars;
using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Gameplay.Cars
{
    public class CarController : MonoBehaviour
    {
        [SerializeField]
        private Transform _visualContainer;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        public Transform Transform => transform;
        public GameObject GameObject => gameObject;

        public void Initialize(CarConfig carConfig)
        {
            SetColor(carConfig.Color);
        }

        public Vector3 GetSpriteSize()
        {
            return _spriteRenderer.bounds.size;
        }

        public void SetDirection(Direction moveDirection)
        {
            int x = 1;
            switch (moveDirection)
            {
                case Direction.Left:
                    x = -1;
                    break;
                case Direction.Right:
                    x = 1;
                    break;
            }

            var scale = _visualContainer.localScale;
            scale.x = x;
            _visualContainer.localScale = scale;
        }

        private void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}