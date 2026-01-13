using UnityEngine;

namespace Project.Scripts.Gameplay.Effects
{
    public abstract class AGameEffect : MonoBehaviour
    {
        [SerializeField]
        protected float _onScreenDuration;

        public float OnScreenDuration => _onScreenDuration;

        public abstract void Play();
    }
}