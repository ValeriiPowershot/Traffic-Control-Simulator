using System;
using System.Collections;
using UnityEngine;

namespace BaseCode.Extensions
{
    public static class CoroutineExtensions
    {
        public static void StopAndNullify(this MonoBehaviour mono, ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                mono.StopCoroutine(coroutine);
                coroutine = null;
            }
        }
        public static Coroutine StartCoroutineWithLoop(this MonoBehaviour mono, Action updateFunction)
        {
            return mono.StartCoroutine(LoopCoroutine(updateFunction));
        }
        
        private static IEnumerator LoopCoroutine(Action updateFunction)
        {
            while (true)
            {
                updateFunction?.Invoke();
                yield return null;
            }
        }
    }

}