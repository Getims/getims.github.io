using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Gameplay.GameFlow
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField]
        private UIEffect _uiEffect;

        private float _switchTime;

        public void Initialize()
        {
            _uiEffect.gradationIntensity = 0;
        }

        public void SetWinBackground()
        {
            DOVirtual
                .Float(0, 1, _switchTime, (x) => { _uiEffect.gradationIntensity = x; })
                .SetLink(gameObject);
        }
    }
}