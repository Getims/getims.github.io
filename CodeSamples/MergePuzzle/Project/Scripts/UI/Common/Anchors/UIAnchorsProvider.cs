using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.UI.Common.Anchors
{
    public interface IUIAnchorsProvider
    {
        void RegisterAnchor(UIAnchorType key, IUIAnchor anchor);
        void RegisterAnchor(UIAnchorType key, Transform point);
        void RegisterAnchor(IUIAnchorTyped anchor);
        Vector3 GetAnchorPosition(UIAnchorType key);
        void RemoveAnchor(UIAnchorType key);
    }

    public class UIAnchorsProvider : IUIAnchorsProvider
    {
        private Dictionary<UIAnchorType, IUIAnchor> _anchors = new();

        public void RegisterAnchor(UIAnchorType key, IUIAnchor anchor)
        {
            if (!_anchors.TryAdd(key, anchor))
                Debug.LogWarning($"UIAnchorsProvider: already has anchor with key {key}");
        }

        public void RegisterAnchor(UIAnchorType key, Transform point)
        {
            var anchor = new MockUIAnchor(point, key);
            RegisterAnchor(anchor);
        }

        public void RegisterAnchor(IUIAnchorTyped anchor)
        {
            var key = anchor.AnchorType;

            if (!_anchors.TryAdd(key, anchor))
                Debug.LogWarning($"UIAnchorsProvider: already has anchor with key {key}");
        }

        public Vector3 GetAnchorPosition(UIAnchorType key)
        {
            if (_anchors.TryGetValue(key, out var anchor))
                if (anchor.AnchorPoint != null)
                    return anchor.AnchorPoint.position;

            Debug.LogWarning($"UIAnchorsProvider: Could not find anchor with key {key}");
            return Vector3.zero;
        }

        public void RemoveAnchor(UIAnchorType key)
        {
            _anchors.Remove(key);
        }
    }
}