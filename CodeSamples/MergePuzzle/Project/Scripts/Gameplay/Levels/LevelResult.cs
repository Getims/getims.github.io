using Coffee.UIEffects;
using Project.Scripts.UIAnimatorModule;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Gameplay.Levels
{
    public class LevelResult : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private Image _backLight;

        [SerializeField]
        private UIAnimator _animator;

        [SerializeField]
        private UIEffectTweener _uiEffect;

        [SerializeField]
        private GameObject _likeButton;

        public void Initialize(Sprite image)
        {
            _image.sprite = image;
            _image.gameObject.SetActive(false);
            _image.enabled = false;
            _uiEffect.enabled = false;
            _likeButton.SetActive(false);
            _backLight.gameObject.SetActive(false);
        }

        public void Show()
        {
            _image.gameObject.SetActive(true);
            _image.enabled = true;
            _animator.Play();
            _uiEffect.enabled = true;
            _likeButton.SetActive(true);
            _backLight.gameObject.SetActive(true);
        }
    }
}