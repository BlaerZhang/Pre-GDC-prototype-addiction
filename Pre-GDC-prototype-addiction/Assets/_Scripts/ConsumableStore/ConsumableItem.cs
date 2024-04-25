using System;
using Interaction.Clickable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace _Scripts.ConsumableStore
{
    public class ConsumableItem : ClickableUIBase
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

            UnlockItem(0);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!isUnlocked) return;
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!isUnlocked) return;
            base.OnPointerExit(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!isUnlocked) return;
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!isUnlocked) return;
            base.OnPointerUp(eventData);
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
                    // change the icon sprite color to white
                    GetComponent<Image>().DOColor(Color.white, 1f);
                    GetComponent<SimpleTooltip>().isEnabled = true;
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