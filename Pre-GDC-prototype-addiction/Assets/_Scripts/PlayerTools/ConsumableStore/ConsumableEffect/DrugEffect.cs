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
        [SerializeField] private float effectDuration = 5f;

        public void Trigger()
        {
            if (drugEffectVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                float endHueShift = effectChangeSpeed * effectDuration;
                DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x, endHueShift, effectDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        float recoverDuration = colorAdjustments.hueShift.value / effectChangeSpeed;
                        DOTween.To(() => colorAdjustments.hueShift.value, x => colorAdjustments.hueShift.value = x,
                                0, recoverDuration)
                            .SetEase(Ease.InOutSine);
                        onDrugEffectStop?.Invoke();
                    });
            }
        }
    }
}