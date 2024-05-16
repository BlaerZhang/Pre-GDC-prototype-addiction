using System;
using _Scripts.Interaction.InteractableUI;
using _Scripts.Manager;
using _Scripts.PlayerTools.ConsumableStore.ConsumableItems;
using _Scripts.PlayerTools.Desk;
using _Scripts.PlayerTools.MembershipSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
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

        private bool isPlayingOutOfStockEffect = false;

        private Image iconImage;

        public AudioClip itemUnlockSound;
        public AudioClip buyItemSound;
        public AudioClip outOfStockSound;

        public static Action<GameObject> onTryBuyItem;

        private void OnEnable()
        {
            MembershipManager.onMembershipLevelUp += UnlockItem;
            ConsumableItemBase.onItemRemoved += Restock;
            iconImage = GetComponent<Image>();
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
            if (isPlayingOutOfStockEffect) return;
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

                GameManager.Instance.audioManager.PlaySound(buyItemSound);

                // if (DeskItemPlacement.isDeskFull) return;
                
                GameManager.Instance.resourceManager.PlayerGold -= price;
                onTryBuyItem?.Invoke(itemBought);
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

            if (newObject.TryGetComponent(out ConsumableItemBase consumableItemBase)) consumableItemBase.consumableType = consumableType;
            else consumablePrefab.AddComponent<ConsumableItemBase>();

            return newObject;
        }
        
        private void OutOfStockEffects()
        {
            if(isPlayingOutOfStockEffect) return;
            isPlayingOutOfStockEffect = true;

            GameManager.Instance.audioManager.PlaySound(outOfStockSound);
            iconImage.DOColor(Color.red, 0.5f).SetEase(Ease.Flash, 4, 0);
            iconImage.rectTransform.DOShakeAnchorPos(0.5f, Vector3.right * 10f).OnComplete(() =>
            {
                isPlayingOutOfStockEffect = false;
            });

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

                    GameManager.Instance.audioManager.PlaySound(itemUnlockSound);
                }
            }
        }

        protected override void ClickableEvent() => BuyItem();
    }
}