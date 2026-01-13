using System.Collections.Generic;
using Lean.Pool;
using Project.Scripts.Core.Events;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Gameplay.Effects
{
    public class EffectsController : MonoBehaviour
    {
        [SerializeField]
        private LeanGameObjectPool _destroyEffectsPool;

        [SerializeField]
        private Transform _effectsContainer;

        [Inject] private GameplayEventProvider _gameplayEventProvider;

        private List<AGameEffect> _effects = new List<AGameEffect>();

        public void Clear()
        {
            foreach (AGameEffect effect in _effects)
                _destroyEffectsPool.Despawn(effect.gameObject);

            _effects.Clear();
        }

        private void Start()
        {
            //_gameplayEventProvider.OnBubbleDestroyEvent.AddListener(OnBubbleDestroyEvent);
        }

        private void OnDestroy()
        {
            //_gameplayEventProvider.OnBubbleDestroyEvent.RemoveListener(OnBubbleDestroyEvent);
        }

        private void PlayEffect()
        {
            var position = Vector3.zero;
            GameObject effectInstance = GetEffectFromPool(_destroyEffectsPool, position);
            var effect = effectInstance.GetComponent<AGameEffect>();
            effect.Play();

            _effects.Add(effect);
            _destroyEffectsPool.Despawn(effectInstance, effect.OnScreenDuration);
        }

        private GameObject GetEffectFromPool(LeanGameObjectPool pool, Vector3 position)
        {
            return pool.Spawn(position, Quaternion.identity, _effectsContainer);
        }
    }
}