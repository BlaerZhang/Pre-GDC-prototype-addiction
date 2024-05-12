using System;
using _Scripts.ConsumableStore.ConsumableEffect;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace _Scripts.ConsumableStore
{
    public class TobaccoItem : ConsumableItemBase
    {
        private void OnEnable()
        {
            TobaccoEffect.onStopSmoking += RemoveItem;
        }

        private void OnDisable()
        {
            TobaccoEffect.onStopSmoking -= RemoveItem;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            // if (isConsuming) return;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            // if (isConsuming) return;
        }

        protected override void ClickableEvent()
        {
            if (isConsuming) return;

            transform.DOScale(1, 0.3f).SetEase(Ease.OutElastic);
            UseItem();
        }
    }
}