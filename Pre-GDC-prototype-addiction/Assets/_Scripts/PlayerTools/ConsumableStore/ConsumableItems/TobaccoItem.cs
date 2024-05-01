using System;
using _Scripts.ConsumableStore.ConsumableEffect;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.ConsumableStore
{
    public class TobaccoItem : ConsumableItemBase
    {
        private Image itemImage;
        private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;

        private void Start()
        {
            itemImage = GetComponent<Image>();
            normalSprite = itemImage.sprite;
        }

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
            if (isConsuming) return;
            itemImage.sprite = hoverSprite;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (isConsuming) return;
            itemImage.sprite = normalSprite;
        }

        protected override void RemoveItem()
        {
            itemImage.sprite = normalSprite;
            base.RemoveItem();
        }

        protected override void ClickableEvent()
        {
            if (isConsuming) return;

            transform.DOScale(1, 0.3f).SetEase(Ease.OutElastic);
            UseItem();
        }
    }
}