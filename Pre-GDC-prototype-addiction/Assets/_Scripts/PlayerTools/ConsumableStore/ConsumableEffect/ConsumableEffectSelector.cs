using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.ConsumableStore.ConsumableEffect
{
    public class ConsumableEffectSelector : SerializedMonoBehaviour
    {
        [DictionaryDrawerSettings(KeyLabel = "Type", ValueLabel = "Effect")]
        [SerializeField] private Dictionary<ConsumableType, IConsumableEffect> effectDict = new();

        private void OnEnable()
        {
            ConsumableItem.onItemConsumed += SelectEffect;
        }

        private void OnDisable()
        {
            ConsumableItem.onItemConsumed -= SelectEffect;
        }

        // TODO: add more effects, remove effects after finished
        private void SelectEffect(ConsumableType consumableType)
        {
            if (effectDict.TryGetValue(consumableType, out IConsumableEffect effect))
            {
                effect.Trigger();
            }
        }
    }
}