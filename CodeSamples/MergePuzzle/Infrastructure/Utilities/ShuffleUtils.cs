using System;
using System.Collections.Generic;

namespace Project.Scripts.Infrastructure.Utilities
{
    public static partial class Utils
    {
        private static Random Random = new Random();

        public static void Shuffle<T>(T[] arr)
        {
            for (int i = arr.Length - 1; i >= 1; i--)
            {
                int j = Random.Next(i + 1);

                T tmp = arr[j];
                arr[j] = arr[i];
                arr[i] = tmp;
            }
        }

        public static void Shuffle<T>(List<T> arr)
        {
            for (int i = arr.Count - 1; i >= 1; i--)
            {
                int j = Random.Next(i + 1);

                T tmp = arr[j];
                arr[j] = arr[i];
                arr[i] = tmp;
            }
        }
    }
}