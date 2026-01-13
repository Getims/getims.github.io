using System;

namespace Project.Scripts.Core.Utilities
{
    public static partial class Utils
    {
        public static int EnumCount<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}