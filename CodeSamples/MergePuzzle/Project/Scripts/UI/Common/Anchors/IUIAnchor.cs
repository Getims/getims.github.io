using UnityEngine;

namespace Project.Scripts.UI.Common.Anchors
{
    public interface IUIAnchor
    {
        Transform AnchorPoint { get; }
    }

    public interface IUIAnchorTyped : IUIAnchor
    {
        UIAnchorType AnchorType { get; }
    }

    public class MockUIAnchor : IUIAnchorTyped
    {
        public Transform AnchorPoint { get; }
        public UIAnchorType AnchorType { get; }

        public MockUIAnchor(Transform position, UIAnchorType type)
        {
            this.AnchorPoint = position;
            this.AnchorType = type;
        }
    }
}