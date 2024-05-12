using System;
using DG.Tweening;
using Interaction.Clickable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.ConsumableStore
{
    public class ConsumableItemBase : InteractableUIBase
    {
        [Title("Item Settings")]
        public ConsumableType consumableType;

        protected Image itemImage;
        protected Sprite normalSprite;
        [SerializeField] protected Sprite hoverSprite;

        public static Action<ConsumableType> onItemConsumed;
        public static Action<string> onItemRemoved;

        protected bool isConsuming = false;

        protected void Awake()
        {
            ConsumableItemIcon.onTryBuyItem += AddItem;
            gameObject.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();
            if (TryGetComponent(out itemImage)) normalSprite = itemImage.sprite;
            else gameObject.AddComponent<Image>();
        }

        protected void OnDestroy()
        {
            ConsumableItemIcon.onTryBuyItem -= AddItem;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (isConsuming) return;
            base.OnPointerEnter(eventData);
            if (hoverSprite) itemImage.sprite = hoverSprite;
            // else Debug.LogError("Hover sprite is null!");
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (isConsuming) return;
            base.OnPointerExit(eventData);
            if (itemImage.sprite) itemImage.sprite = normalSprite;
            else Debug.LogError("itemImage is null!");
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
            itemImage.sprite = normalSprite;

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