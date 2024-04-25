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

        private bool isUnlocked = false;

        public void InitializeItem(string itemName, ConsumableType consumableType, int unlockLevel, float price, string description)
        {
            this.itemName = itemName;
            this.consumableType = consumableType;
            this.unlockLevel = unlockLevel;
            this.price = price;
            this.description = description;
        }

        private void UseEffect()
        {
            if (isUnlocked) onItemConsumed(consumableType);
        }

        // TODO: triggered after upgrade the membership level
        private void UnlockItem(int currentLevel)
        {
            if (currentLevel == unlockLevel)
            {
                if (!isUnlocked)
                {
                    isUnlocked = true;

                }
            }

        }

        private void ChangeToUnlockState()
        {

        }

        protected override void ClickableEvent() => UseEffect();
    }
}