using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.ConsumableStore
{
    public class ConsumableStoreInitializer : MonoBehaviour
    {
        [Title("Consumable Item Data")]
        [SerializeField] private ConsumableItemsData consumableItemsData;

        [SerializeField] private GameObject consumablePrefab;

        [Title("Item Placement")]
        [SerializeField] private GridLayoutGroup consumableStore;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private Vector2 gapLength;
        [SerializeField] private int gridLayoutSizeConstraint;

        [Title("Price Text Settings")]
        [SerializeField] private Vector2 priceTextOffset = new Vector2(0, -60f);
        [SerializeField] private Color priceTextColor = Color.black;
        [SerializeField] private TMP_FontAsset priceTextFont;
        [SerializeField] private int priceTextFontSize = 36;

        [Title("Unlock Level Text Settings")]
        [SerializeField] private Vector2 unlockLevelTextOffset = Vector2.zero;
        [SerializeField] private Color unlockLevelTextColor = Color.black;
        [SerializeField] private TMP_FontAsset unlockLevelTextFont;
        [SerializeField] private int unlockLevelTextFontSize = 36;
        [SerializeField] private GameObject lockedImagePrefab;

        // TODO: locked sprite for consumable

        [Title("Item Tooltip Settings")]
        [SerializeField] private SimpleTooltipStyle tooltipStyle;

        void Start()
        {
            SetUpGridLayout();
            PlaceItems();
        }

        private void SetUpGridLayout()
        {
            consumableStore.startCorner = GridLayoutGroup.Corner.UpperLeft;
            consumableStore.startAxis = GridLayoutGroup.Axis.Vertical;
            consumableStore.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            consumableStore.constraintCount = gridLayoutSizeConstraint;

            consumableStore.cellSize = cellSize;
            consumableStore.spacing = gapLength;
        }

        /// <summary>
        /// assemble items from the data list
        /// </summary>
        private GameObject AssembleItemIcons(string itemName, Sprite icon, Sprite itemSprite, ConsumableType consumableType, int unlockLevel, int price, string description)
        {
            GameObject consumableItem = new GameObject(itemName);
            
            var image = consumableItem.AddComponent<Image>();
            // // image.SetNativeSize();
            image.sprite = icon;
            // image.color = Color.black;
            GameObject lockMask;
            if (lockedImagePrefab) lockMask = Instantiate(lockedImagePrefab, image.transform);

            // add price, unlock level to each icon
            AddPriceText(consumableItem.transform, price);
            AddUnlockLevelText(consumableItem.transform, unlockLevel);

            // add tooltip on consumable items
            AddTooltip(consumableItem, itemName, description, price);

            consumableItem.AddComponent<ConsumableItemIcon>().InitializeItem(consumablePrefab, itemName, price, itemSprite, consumableType, unlockLevel);

            return consumableItem;
        }

        private void AddPriceText(Transform parent, int price)
        {
            GameObject priceTextObject = new GameObject("priceText");
            priceTextObject.transform.SetParent(parent);
            priceTextObject.transform.position += (Vector3)priceTextOffset;
            var textMeshPro = priceTextObject.AddComponent<TextMeshProUGUI>();
            textMeshPro.text = price.ToString();

            textMeshPro.color = priceTextColor;
            textMeshPro.font = priceTextFont;
            textMeshPro.fontSize = priceTextFontSize;
            textMeshPro.alignment = TextAlignmentOptions.Center;
        }

        private void AddUnlockLevelText(Transform parent, int unlockLevel)
        {
            GameObject priceTextObject = new GameObject("unlockLevelText");
            priceTextObject.transform.SetParent(parent);
            priceTextObject.transform.position += (Vector3)unlockLevelTextOffset;
            var textMeshPro = priceTextObject.AddComponent<TextMeshProUGUI>();
            textMeshPro.text = unlockLevel.ToString();

            textMeshPro.color = unlockLevelTextColor;
            textMeshPro.font = unlockLevelTextFont;
            textMeshPro.fontSize = unlockLevelTextFontSize;
            textMeshPro.alignment = TextAlignmentOptions.Center;
        }

        /// <summary>
        /// ~ for title; ^ for default text; @ for price
        /// </summary>
        private void AddTooltip(GameObject consumableItem, string itemName, string description, int price)
        {
            var tooltip = consumableItem.AddComponent<SimpleTooltip>();
            tooltip.isEnabled = false;

            if (tooltipStyle) tooltip.simpleTooltipStyle = tooltipStyle;
            tooltip.infoLeft += "~" + itemName + "\n";
            tooltip.infoLeft += "^" + description;
            tooltip.infoRight += "@$" + price;
        }

        /// <summary>
        /// place the items on the shelf
        /// </summary>
        private void PlaceItems()
        {
            int totalItemNumber = consumableItemsData.consumableItemsDataList.Count;

            for (int i = 0; i < totalItemNumber; i++)
            {
                var currentDataElement = consumableItemsData.consumableItemsDataList[i];
                GameObject currentItem = AssembleItemIcons(currentDataElement.name, currentDataElement.icon, currentDataElement.itemSprite, currentDataElement.type, currentDataElement.unlockLevel, currentDataElement.price, currentDataElement.description);

                currentItem.transform.SetParent(consumableStore.transform);
                currentItem.transform.localScale = Vector3.one;
            }
        }
    }
}
