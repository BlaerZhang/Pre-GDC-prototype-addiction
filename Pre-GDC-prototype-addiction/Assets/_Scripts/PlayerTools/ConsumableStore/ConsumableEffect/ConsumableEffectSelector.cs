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
            ConsumableItemBase.onItemConsumed += SelectEffect;
        }

        private void OnDisable()
        {
            ConsumableItemBase.onItemConsumed -= SelectEffect;
        }

        // TODO: remove effects after finished?
        private void SelectEffect(ConsumableType consumableType)
        {
            if (effectDict.TryGetValue(consumableType, out IConsumableEffect effect))
            {
                effect.Trigger();
            }
        }
    }
}