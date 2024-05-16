using System;
using _Scripts.Interaction.InteractableUI;
using _Scripts.Manager;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.PlayerTools.ConsumableStore.ConsumableItems
{
    public class ConsumableItemBase : InteractableUIBase
    {
        public AudioClip itemGenerateSound;

        [Title("Item Settings")]
        public ConsumableType consumableType;

        protected Image itemImage;
        protected Sprite normalSprite;
        [SerializeField] protected Sprite hoverSprite;

        public static Action<ConsumableType> onItemConsumed;
        public static Action<string> onItemRemoved;

        protected bool isDisabled = false;

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
            if (isDisabled) return;
            base.OnPointerEnter(eventData);
            if (hoverSprite) itemImage.sprite = hoverSprite;
            // else Debug.LogError("Hover sprite is null!");
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (isDisabled) return;
            base.OnPointerExit(eventData);
            if (itemImage.sprite) itemImage.sprite = normalSprite;
            else Debug.LogError("itemImage is null!");
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (isDisabled) return;
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (isDisabled) return;
            base.OnPointerUp(eventData);
        }


        private void AddItem(GameObject item)
        {
            if (!item.name.Equals(name)) return;

            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 originalPosition = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + 250);
            gameObject.SetActive(true);

            isDisabled = true;
            GameManager.Instance.audioManager.PlaySound(itemGenerateSound);
            rectTransform.DOAnchorPosY(originalPosition.y, 0.5f).SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    isDisabled = false;
                });
        }

        protected void UseItem()
        {
            isDisabled = true;

            print($"{name} is used");
            onItemConsumed?.Invoke(consumableType);
        }

        protected virtual void RemoveItem()
        {
            itemImage.sprite = normalSprite;

            isDisabled = false;

            gameObject.SetActive(false);
            print($"{name} is removed");
            onItemRemoved?.Invoke(gameObject.name);
        }

        protected override void ClickableEvent()
        {
            if (isDisabled) return;
            transform.DOScale(1, 0.3f).SetEase(Ease.OutElastic);
            UseItem();
            RemoveItem();
        }
    }
}