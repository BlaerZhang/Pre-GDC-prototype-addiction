using System;
using _Scripts.PlayerTools;
using Interaction.Clickable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Manager;
using TMPro;

namespace _Scripts.ConsumableStore
{
    // TODO: cannot buy if already got one
    public class ConsumableItem : ClickableUIBase, IUnlockable
    {
        private ConsumableType consumableType;
        private int unlockLevel;
        private int price;
        private string description;

        public static Action<ConsumableType> onItemConsumed;

        private bool isUnlocked = false;

        private void OnEnable()
        {
            MembershipManager.onMembershipLevelUp += UnlockItem;
        }

        private void OnDisable()
        {
            MembershipManager.onMembershipLevelUp -= UnlockItem;
        }

        public void InitializeItem(ConsumableType consumableType, int unlockLevel)
        {
            this.consumableType = consumableType;
            this.unlockLevel = unlockLevel;

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

        public void UnlockItem(int currentLevel)
        {
            if (currentLevel >= unlockLevel)
            {
                if (!isUnlocked)
                {
                    // change the icon sprite color to white
                    GetComponent<Image>().DOColor(Color.white, 1f);
                    transform.Find("unlockLevelText").gameObject.SetActive(false);
                    GetComponent<SimpleTooltip>().isEnabled = true;
                    isUnlocked = true;
                }
            }
        }

        protected override void ClickableEvent()
        {
            UseEffect();
        }
    }
}