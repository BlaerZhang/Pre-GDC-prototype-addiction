using System;
using Interaction.Clickable;
using UnityEngine;

namespace _Scripts.ConsumableStore
{
    public class ConsumableItem : ClickableBase
    {
        private string itemName;
        private ConsumableType consumableType;
        private int unlockLevel;
        private float price;
        private string description;

        public static Action<ConsumableType> onItemConsumed;

        public void InitializeItem(string itemName, ConsumableType consumableType, int unlockLevel, float price, string description)
        {
            this.itemName = itemName;
            this.consumableType = consumableType;
            this.unlockLevel = unlockLevel;
            this.price = price;
            this.description = description;
        }

        // TODO: triggered after the drag function
        private void UseEffect() => onItemConsumed(consumableType);

        protected override void ClickableEvent() => UseEffect();
    }
}