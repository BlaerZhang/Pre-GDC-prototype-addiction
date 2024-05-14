using System;
using _Scripts.Interaction.InteractableUI;
using _Scripts.Manager;
using _Scripts.PlayerTools.ConsumableStore.ConsumableItems;
using _Scripts.PlayerTools.Desk;
using _Scripts.PlayerTools.MembershipSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.PlayerTools.ConsumableStore
{
    public class ConsumableItemIcon : InteractableUIBase, IUnlockable
    {
        private GameObject consumablePrefab;
        private string itemName;
        private Sprite itemSprite;
        private ConsumableType consumableType;
        private int unlockLevel;
        private int price;
        private string description;

        private bool isUnlocked = false;
        private bool isOutOfStock = false;

        public static Action<GameObject> onTryBuyItem;

        private void OnEnable()
        {
            MembershipManager.onMembershipLevelUp += UnlockItem;
            ConsumableItemBase.onItemRemoved += Restock;
        }

        private void OnDisable()
        {
            MembershipManager.onMembershipLevelUp -= UnlockItem;
            ConsumableItemBase.onItemRemoved -= Restock;
        }

        private void Restock(string usedItemName)
        {
            if(usedItemName.Equals(itemName)) isOutOfStock = false;
        }

        public void InitializeItem(GameObject consumablePrefab, string itemName, int price, Sprite itemSprite, ConsumableType consumableType, int unlockLevel, AudioClip pressSound)
        {
            this.consumablePrefab = consumablePrefab;
            this.itemName = itemName;
            this.price = price;
            this.itemSprite = itemSprite;
            this.consumableType = consumableType;
            this.unlockLevel = unlockLevel;
            this.pressSounds.Add(pressSound);

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

        private void BuyItem()
        {
            if (isUnlocked & GameManager.Instance.resourceManager.PlayerGold >= price)
            {
                if (isOutOfStock)
                {
                    print($"{itemName} is out of stock!");
                    OutOfStockEffects();
                    return;
                }

                // check if there is enough space on desk, then decide whether successfully bought an item
                GameObject itemBought = AssembleItemObject(consumablePrefab);
                onTryBuyItem?.Invoke(itemBought);

                if (DeskItemPlacement.isDeskFull) return;
                
                GameManager.Instance.resourceManager.PlayerGold -= price;
                isOutOfStock = true;
            }
        }

        private GameObject AssembleItemObject(GameObject newObject)
        {
            newObject.name = itemName;

            if (newObject.TryGetComponent(out Image image))
            {
                image.sprite = itemSprite;
                image.SetNativeSize();
            }
            else
            {
                Image newImage = newObject.AddComponent<Image>();
                newImage.sprite = itemSprite;
                newImage.SetNativeSize();
            }

            // TODO: select specific item script according to the consumable type
            if (newObject.TryGetComponent(out ConsumableItemBase consumableItemBase)) consumableItemBase.consumableType = consumableType;
            else consumablePrefab.AddComponent<ConsumableItemBase>();

            return newObject;
        }

        // TODO: OutOfStockEffects
        private void OutOfStockEffects()
        {

        }

        public void UnlockItem(int currentLevel)
        {
            if (currentLevel >= unlockLevel)
            {
                if (!isUnlocked)
                {
                    // change the icon sprite color to white
                    transform.GetChild(0).GetComponent<Image>().DOColor(Color.clear, 1f);
                    // transform.Find("unlockLevelText").gameObject.SetActive(false);
                    GetComponent<SimpleTooltip>().isEnabled = true;
                    isUnlocked = true;
                }
            }
        }

        protected override void ClickableEvent() => BuyItem();
    }
}