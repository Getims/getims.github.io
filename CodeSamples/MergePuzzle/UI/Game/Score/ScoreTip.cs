using Project.Scripts.Modules.UIAnimator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI.Game.Score
{
    public class ScoreTip : MonoBehaviour
    {
        [SerializeField]
        private UIAnimator _animator;

        [SerializeField]
        private Image _spriteImage;

        [SerializeField]
        private TMP_Text _textField;

        private bool _isAnimating;

        public bool IsAnimating => _isAnimating;

        public void Initialize(Sprite sprite)
        {
            _spriteImage.sprite = sprite;
            _spriteImage.enabled = true;
            _textField.enabled = false;
        }

        public void Initialize(int score)
        {
            _textField.text = $"+{score}";
            _textField.enabled = true;
            _spriteImage.enabled = false;
        }

        public void SetPosition(Vector2 localPosition)
        {
            transform.position = localPosition;
        }

        public void Show()
        {
            _isAnimating = true;
            _animator.Play();
        }

        public void Hide()
        {
            _isAnimating = false;
            gameObject.SetActive(false);
        }

        private void Start()
        {
            _animator.OnComplete += OnAnimationComplete;
        }

        private void OnAnimationComplete()
        {
            _isAnimating = false;
        }
    }
}