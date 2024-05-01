using System;
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

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            itemImage.sprite = hoverSprite;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            itemImage.sprite = normalSprite;
        }
    }
}