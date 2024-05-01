using System;
using System.Collections;
using Manager;
using UnityEngine;

namespace _Scripts.ConsumableStore.ConsumableEffect
{
    public class TobaccoEffect : MonoBehaviour, IConsumableEffect
    {
        public static Action onStopSmoking;

        [SerializeField] private float effectDuration = 5f;

        public void Trigger()
        {
            // Debug.LogError("tobacco effect not implemented");
            StartCoroutine(StartSmoking());
        }

        private IEnumerator StartSmoking()
        {
            // smoke covers the screen, disappear after duration

            // start effect countdown, interactions will not consume time in this duration
            GameManager.Instance.resourceManager.isTimeStopped = true;
            print("start smoking");
            yield return new WaitForSeconds(effectDuration);
            print("stop smoking");
            GameManager.Instance.resourceManager.isTimeStopped = false;

            // when the effect ends, tell ashtray generator to generate a new stub
            onStopSmoking?.Invoke();
        }
    }
}