using System;
using _Scripts.ConsumableStore.ConsumableEffect;
using DG.Tweening;
using Interaction.Clickable;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.ConsumableStore
{
    public class ConsumableItemBase : InteractableUIBase
    {
        [Header("Item Settings")]
        public ConsumableType consumableType;

        public static Action<ConsumableType> onItemConsumed;
        public static Action<string> onItemRemoved;

        protected bool isConsuming = false;

        protected void Awake()
        {
            ConsumableItemIcon.onTryBuyItem += AddItem;
            gameObject.SetActive(false);
        }

        protected void OnDestroy()
        {
            ConsumableItemIcon.onTryBuyItem -= AddItem;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (isConsuming) return;
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (isConsuming) return;
            base.OnPointerExit(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (isConsuming) return;
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (isConsuming) return;
            base.OnPointerUp(eventData);
        }


        private void AddItem(GameObject item)
        {
            if (!item.name.Equals(name)) return;

            gameObject.SetActive(true);
        }

        protected void UseItem()
        {
            isConsuming = true;

            print($"{name} is used");
            onItemConsumed?.Invoke(consumableType);
        }

        protected virtual void RemoveItem()
        {
            isConsuming = false;

            gameObject.SetActive(false);
            print($"{name} is removed");
            onItemRemoved?.Invoke(gameObject.name);
        }

        protected override void ClickableEvent()
        {
            if (isConsuming) return;
            transform.DOScale(1, 0.3f).SetEase(Ease.OutElastic);
            UseItem();
            RemoveItem();
        }
    }
}