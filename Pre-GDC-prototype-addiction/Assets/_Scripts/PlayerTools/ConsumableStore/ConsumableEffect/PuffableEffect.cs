using System;
using System.Collections;
using Manager;
using UnityEngine;

namespace _Scripts.ConsumableStore.ConsumableEffect
{
    public class PuffableEffect : MonoBehaviour, IConsumableEffect
    {
        public static Action onStopSmoking;

        [SerializeField] private ParticleSystem smokeParticleSystemPrefab;
        [SerializeField] private Vector3 smokeGeneratePosition;
        [SerializeField] private float effectDuration = 5f;

        private GameObject smokeParticleObject;

        public void Trigger()
        {
            // Debug.LogError("tobacco effect not implemented");
            StartCoroutine(StartSmoking());
        }

        private IEnumerator StartSmoking()
        {
            // smoke covers the screen, disappear after duration
            if (!smokeParticleObject)
            {
                if (smokeParticleSystemPrefab)
                {
                    smokeParticleObject = Instantiate(smokeParticleSystemPrefab.gameObject, smokeGeneratePosition,
                        Quaternion.identity);
                    smokeParticleObject.transform.localScale /= 2;
                    var mainModule = smokeParticleSystemPrefab.main;
                    mainModule.duration = effectDuration;
                    mainModule.loop = false;
                    mainModule.playOnAwake = true;
                }
            }
            else
            {
                smokeParticleObject.GetComponent<ParticleSystem>()?.Play();
            }

            // start effect countdown, interactions will not consume time in this duration
            GameManager.Instance.resourceManager.isTimeStopped = true;
            print("start smoking");
            yield return new WaitForSeconds(effectDuration);
            print("stop smoking");
            smokeParticleObject.GetComponent<ParticleSystem>()?.Stop();
            GameManager.Instance.resourceManager.isTimeStopped = false;

            // when the effect ends, tell ashtray generator to generate a new stub
            onStopSmoking?.Invoke();
        }
    }
}