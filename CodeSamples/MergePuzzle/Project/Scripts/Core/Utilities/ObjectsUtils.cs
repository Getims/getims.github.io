using System;
using UnityEngine;

namespace Project.Scripts.Core.Utilities
{
    public static partial class Utils
    {
        public static string GetUniqueID() =>
            Guid.NewGuid().ToString().Remove(0, 20);

        public static string GetUniqueID(int length)
        {
            string randomString = Guid.NewGuid().ToString();
            if (length < randomString.Length)
                randomString = randomString.Substring(0, length);
            return randomString;
        }

        public static void SetLocalSize(this Transform transform, float size)
        {
            transform.localScale = Vector3.one * size;
        }

        public static void SetLocalPositionY(this Transform transform, float position)
        {
            var localPosition = transform.localPosition;
            transform.localPosition = new Vector3(localPosition.x, position, localPosition.z);
        }

        public static void SetLocalPositionX(this Transform transform, float position)
        {
            var localPosition = transform.localPosition;
            transform.localPosition = new Vector3(position, localPosition.y, localPosition.z);
        }

        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
        {
            var anchoredPosition = rectTransform.anchoredPosition;
            anchoredPosition.x = x;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        public static void SetLocalPositionZ(this Transform transform, float position)
        {
            var localPosition = transform.localPosition;
            transform.localPosition = new Vector3(localPosition.x, localPosition.y, position);
        }
    }
}