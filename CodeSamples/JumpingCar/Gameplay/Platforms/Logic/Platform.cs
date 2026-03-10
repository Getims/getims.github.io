using DG.Tweening;
using Project.Scripts.Configs.Platforms;
using Project.Scripts.Runtime.Constants;
using Project.Scripts.Runtime.Enums;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Platforms.Logic
{
    public interface IPlatform
    {
        GameObject gameObject { get; }
        float TopY { get; }
        bool IsActive { get; }
        Vector3 Position { get; }
        int Number { get; }

        void SetState(PlatformsState state);
        void ResetState();
        Tween DOMove(Vector3 targetPosition, float moveDuration);
    }

    public class Platform : MonoBehaviour, IPlatform
    {
        [SerializeField]
        private BoxCollider2D _boxCollider2D;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Transform _spriteContainer;

        [SerializeField]
        private TMP_Text _numberTMP;

        [SerializeField]
        private TMP_Text _infoTMP;

        private bool _playerTouched;
        private ColorsConfig _colorsConfig;
        private int _number;

        public float TopY => transform.position.y + _spriteRenderer.bounds.size.y * 0.5f;
        public bool IsActive => isActiveAndEnabled;
        public Vector3 Position => transform.position;
        public int Number => _number;

        public void Initialize(ColorsConfig colorsConfig, int number)
        {
            _colorsConfig = colorsConfig;
            _number = number;
            _numberTMP.text = number.ToString();
        }

        public void SetWidth(float width)
        {
            _spriteContainer.localScale =
                new Vector3(width, _spriteContainer.localScale.y, _spriteContainer.localScale.z);
            _boxCollider2D.size = new Vector2(width, 1);

            var textSize = _numberTMP.rectTransform.sizeDelta;
            textSize.x = width;

            _numberTMP.rectTransform.sizeDelta = textSize;
            _infoTMP.rectTransform.sizeDelta = textSize;
        }

        public void SetState(PlatformsState state)
        {
            var spriteColor = Color.white;
            var textColor = Color.white;

            switch (state)
            {
                case PlatformsState.Current:
                    spriteColor = _colorsConfig.CurrentPlatformColor;
                    break;
                case PlatformsState.Next:
                    spriteColor = _colorsConfig.NextPlatformColor;
                    break;
                case PlatformsState.Special:
                    spriteColor = _colorsConfig.InfoPlatformColor;
                    break;
            }

            _spriteRenderer.color = spriteColor;

            textColor = textColor * 0.65f + spriteColor * 0.35f;
            _numberTMP.color = textColor;
            _infoTMP.color = textColor;
        }

        public void SetInfo(string info)
        {
            _infoTMP.text = info;
        }

        public void ResetState()
        {
            _playerTouched = false;
            _numberTMP.text = string.Empty;
            _infoTMP.text = string.Empty;
        }

        public Tween DOMove(Vector3 targetPosition, float moveDuration)
        {
            return transform.DOMove(targetPosition, moveDuration);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_playerTouched && collision.gameObject.CompareTag(GameTags.PLAYER))
            {
                _playerTouched = true;
                _spriteRenderer.color = _colorsConfig.CurrentPlatformColor;
            }
        }
    }
}