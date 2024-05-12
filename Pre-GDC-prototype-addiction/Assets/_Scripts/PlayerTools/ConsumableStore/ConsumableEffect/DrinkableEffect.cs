using System;
using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ConsumableStore.ConsumableEffect
{
    public class DrinkableEffect : MonoBehaviour, IConsumableEffect
    {
        [SerializeField] private ParticleSystem waterFountainParticleSystemPrefab;
        [SerializeField] private Vector3 waterFountainGeneratePosition;
        [SerializeField] private float effectDuration;

        private GameObject waterFountainParticleObject;

        public void Trigger()
        {
            print("drinkable effect triggered");

            StartCoroutine(StartDrinking());
        }

        private IEnumerator StartDrinking()
        {
            // smoke covers the screen, disappear after duration
            if (!waterFountainParticleObject)
            {
                if (waterFountainParticleSystemPrefab)
                {
                    waterFountainParticleObject = Instantiate(waterFountainParticleSystemPrefab.gameObject, waterFountainGeneratePosition,
                        Quaternion.identity);
                    var mainModule = waterFountainParticleSystemPrefab.main;
                    mainModule.duration = effectDuration;
                    mainModule.loop = false;
                    mainModule.playOnAwake = true;
                }
            }
            else
            {
                waterFountainParticleObject.GetComponent<ParticleSystem>()?.Play();
            }

            // start effect countdown, interactions will not consume time in this duration
            print("start spray");
            yield return new WaitForSeconds(effectDuration);
            print("stop spray");
            waterFountainParticleObject.GetComponent<ParticleSystem>()?.Stop();
        }
    }
}