using System;
using _Scripts.ConsumableStore.ConsumableEffect;
using Interaction.Clickable;
using UnityEngine;

namespace _Scripts.ConsumableStore
{
    public class ConsumableItemBase : ClickableUIBase
    {
        [HideInInspector] public ConsumableType consumableType;

        public static Action<ConsumableType> onItemConsumed;
        public static Action<string> onItemRemoved;

        private void UseItem()
        {
            print($"{name} is used");
            onItemConsumed?.Invoke(consumableType);

            RemoveItem();
        }

        private void RemoveItem()
        {
            print($"{name} is removed");
            onItemRemoved?.Invoke(gameObject.name);
        }

        protected override void ClickableEvent() => UseItem();
    }
}