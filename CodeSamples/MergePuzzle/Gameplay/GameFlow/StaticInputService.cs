namespace Project.Scripts.Gameplay.GameFlow
{
    public static class StaticInputService
    {
        private static bool _isAnimation;
        private static bool _isDragging;

        public static bool CanDrag => !_isDragging && !_isAnimation;

        public static void SetDragBlock(bool enabled)
        {
            _isDragging = enabled;
        }

        public static void SetAnimationBlock(bool enabled)
        {
            _isAnimation = enabled;
        }
    }
}