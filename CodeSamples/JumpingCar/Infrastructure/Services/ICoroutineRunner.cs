using System.Collections;
using UnityEngine;

namespace Project.Scripts.Infrastructure.Services
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
        void StopCoroutine(Coroutine routine);
    }
}