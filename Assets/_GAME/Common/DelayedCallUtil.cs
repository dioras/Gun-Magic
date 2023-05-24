using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace _GAME
{
    public static class DelayedCallUtil
    {
        public static Coroutine DelayedCall(this MonoBehaviour component, float delay, Action action, bool ignoreTimeScale = false)
        {
            return component.StartCoroutine(Invoke(delay, action, ignoreTimeScale));
        }

        public static IEnumerator Invoke(float t, Action action, bool ignoreTimeScale = false)
        {
            if (ignoreTimeScale)
                yield return new WaitForSecondsRealtime(t);
            else
                yield return new WaitForSeconds(t);

            action?.Invoke();
        }

        public static async void DelayedCall(float delay, Action action, bool ignoreTimeScale = false)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));

            action?.Invoke();
        }
    }
}