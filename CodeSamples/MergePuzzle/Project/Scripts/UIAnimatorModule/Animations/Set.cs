using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UIAnimatorModule.Animations
{
    [Serializable]
    public class Set
    {
        [SerializeField]
        private Type _type;

        [SerializeField]
        private bool _isActive;

        [SerializeField]
        [ShowIf(nameof(_type), Type.GameObject)]
        private GameObject _gameObject;

        [SerializeField]
        [ShowIf(nameof(_type), Type.Image)]
        private Image _image;

        public string GetObjectName()
        {
            GameObject gameObject = GetObject();
            if (gameObject != null)
                return gameObject.name;
            else
                return string.Empty;
        }

        public Tween GetTween(float animationTime, bool instant)
        {
            switch (_type)
            {
                case Type.GameObject:
                    return GetForGameObject(instant ? 0 : animationTime);
                case Type.Image:
                    return GetForImage(instant ? 0 : animationTime);
            }

            return null;
        }

        private Tween GetForGameObject(float time)
        {
            return DOVirtual.DelayedCall(time, () => _gameObject.SetActive(_isActive));
        }

        private Tween GetForImage(float time)
        {
            return DOVirtual.DelayedCall(time, () => _image.enabled = _isActive);
        }

        private GameObject GetObject()
        {
            switch (_type)
            {
                case Type.GameObject:
                    return _gameObject != null ? _gameObject : null;
                case Type.Image:
                    return _image != null ? _image.gameObject : null;
            }

            return null;
        }

        private enum Type
        {
            GameObject,
            Image
        }
    }
}