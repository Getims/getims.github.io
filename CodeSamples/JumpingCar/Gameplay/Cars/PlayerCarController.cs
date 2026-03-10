using System;
using UnityEngine;

namespace Project.Scripts.Gameplay.Cars
{
    public class PlayerCarController : CarController
    {
        [SerializeField]
        private GameObject _explosionPS;

        public event Action<Collision2D> OnCollision;

        public void ShowExplosion()
        {
            _explosionPS.SetActive(true);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            OnCollision?.Invoke(other);
        }
    }
}