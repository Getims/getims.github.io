using Project.Scripts.Runtime.Enums;
using UnityEngine;

namespace Project.Scripts.Runtime
{
    public static class RuntimeUtils
    {
        public static Direction RandomDirection(Direction value1, Direction value2)
        {
            if (Random.value > 0.5f)
                return value1;
            else
                return value2;
        }
    }
}