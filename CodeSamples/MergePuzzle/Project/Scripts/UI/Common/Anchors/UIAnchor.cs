using UnityEngine;
using Zenject;

namespace Project.Scripts.UI.Common.Anchors
{
    public class UIAnchor : MonoBehaviour, IUIAnchorTyped
    {
        [SerializeField]
        private Transform _point;

        [SerializeField]
        private UIAnchorType _anchorType;

        [SerializeField]
        private bool _useOnEnable = false;

        [Inject] IUIAnchorsProvider _anchorsProvider;

        public Transform AnchorPoint => _point;
        public UIAnchorType AnchorType => _anchorType;

        private void Start()
        {
            if (_anchorType == UIAnchorType.NULL)
                return;

            _anchorsProvider.RegisterAnchor(this);
            gameObject.name += $" {_anchorType}";
        }

        private void OnDestroy()
        {
            _anchorsProvider.RemoveAnchor(_anchorType);
        }

        private void OnEnable()
        {
            if (_useOnEnable)
                Start();
        }

        private void OnDisable()
        {
            if (_useOnEnable)
                OnDestroy();
        }
    }
}