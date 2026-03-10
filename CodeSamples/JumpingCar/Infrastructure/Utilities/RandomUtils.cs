using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scripts.Infrastructure.Utilities
{
    public static partial class Utils
    {
        public static int RandomValue(int value1, int value2)
        {
            if (UnityEngine.Random.value > 0.5f)
                return value1;
            else
                return value2;
        }

        public static int RandomValue(params int[] values)
        {
            if (values == null || values.Length == 0)
                return 0;

            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        public static float RandomValue(params float[] values)
        {
            if (values == null || values.Length == 0)
                return 0;

            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        public static Vector3 RandomValue(params Vector3[] values)
        {
            if (values == null || values.Length == 0)
                return Vector3.zero;

            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        public static float RandomValue(float value1, float value2)
        {
            if (UnityEngine.Random.value > 0.5f)
                return value1;
            else
                return value2;
        }

        public static bool RandomBool()
        {
            if (UnityEngine.Random.value > 0.5f)
                return true;
            else
                return false;
        }

        public static int RandomValueExcluding(int min, int max, int exclude)
        {
            int randomNumber = UnityEngine.Random.Range(min, max);
            if (randomNumber == exclude)
                randomNumber = (randomNumber + 1) % max;

            return randomNumber;
        }

        public static bool RandomChance(int chance)
        {
            if (chance <= 0)
                return false;
            if (chance >= 100)
                return true;

            int randomNumber = UnityEngine.Random.Range(0, 100);

            return randomNumber <= chance;
        }

        public static T GetRandomElement<T>(this IEnumerable<T> container) =>
            container.ElementAt(UnityEngine.Random.Range(0, container.Count()));
    }
}