using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace _Scripts.ConsumableStore.ConsumableEffect
{
    public class DrugEffect : MonoBehaviour, IConsumableEffect
    {
        public static Action onDrugEffectStop;

        [SerializeField] private Volume drugEffectVolume;
        [SerializeField] private float maxHueShift = 180.0f;
        [SerializeField] private float effectChangeSpeed;
        [SerializeField] private float effectDuration = 10f;
        [SerializeField] private float recoverDuration = 1f;

        public void Trigger()
        {
            if (drugEffectVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                Tween hueShiftTween = DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = Mathf.Sin(x * effectChangeSpeed * Mathf.PI) * maxHueShift, effectDuration, effectDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        // float recoverDuration = colorAdjustments.hueShift.value / effectChangeSpeed;
                        DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x,
                                0, recoverDuration)
                            .SetEase(Ease.InOutSine);
                        onDrugEffectStop?.Invoke();
                    });

                 hueShiftTween.Play();
            }
        }
    }
}