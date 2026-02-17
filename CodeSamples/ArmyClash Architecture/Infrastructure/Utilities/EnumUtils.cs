using System;

namespace Project.Scripts.Infrastructure.Utilities
{
    public static partial class Utils
    {
        public static int EnumCount<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}