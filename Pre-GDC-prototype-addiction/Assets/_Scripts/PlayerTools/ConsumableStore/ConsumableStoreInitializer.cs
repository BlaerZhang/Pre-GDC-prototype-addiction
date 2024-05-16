using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.PlayerTools.ConsumableStore
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

        [Title("Locked Settings")]
        [SerializeField] private GameObject lockedImagePrefab;

        [Title("Item Tooltip Settings")]
        [SerializeField] private SimpleTooltipStyle tooltipStyle;

        [Title("Item Audio Settings")]
        [SerializeField] private AudioClip itemUnlockSound;
        [SerializeField] private AudioClip buyItemSound;
        [SerializeField] private AudioClip outOfStockSound;

        void Start()
        {
            SetUpGridLayout();
            PlaceItems();
        }

        private void SetUpGridLayout()
        {
            consumableStore.startCorner = GridLayoutGroup.Corner.UpperLeft;
            consumableStore.startAxis = GridLayoutGroup.Axis.Horizontal;
            consumableStore.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            consumableStore.constraintCount = gridLayoutSizeConstraint;

            consumableStore.cellSize = cellSize;
            consumableStore.spacing = gapLength;
        }

        /// <summary>
        /// assemble items from the data list
        /// </summary>
        private GameObject AssembleItemIcons(string itemName, Sprite icon, Sprite itemSprite, ConsumableType consumableType, int unlockLevel, int price, string itemDescription, ConsumableItemsData.LockedInformation lockedInformation)
        {
            GameObject consumableItem = new GameObject(itemName);
            
            var image = consumableItem.AddComponent<Image>();
            // // image.SetNativeSize();
            image.sprite = icon;
            // image.color = Color.black;
            if (lockedImagePrefab)
            {
                GameObject lockedObject = Instantiate(lockedImagePrefab, image.transform);
                AddTooltip(lockedObject, lockedInformation.leftCornerText, lockedInformation.lockedDescription, lockedInformation.rightCornerText, true);
            }

            // add tooltip on consumable items
            AddTooltip(consumableItem, itemName, itemDescription, price.ToString());

            var consumableIcon = consumableItem.AddComponent<ConsumableItemIcon>();
            consumableIcon.InitializeItem(consumablePrefab, itemName, price, itemSprite, consumableType, unlockLevel, buyItemSound);
            consumableIcon.buyItemSound = buyItemSound;
            consumableIcon.itemUnlockSound = itemUnlockSound;
            consumableIcon.outOfStockSound = outOfStockSound;

            return consumableItem;
        }

        /// <summary>
        /// ~ for title; ^ for default text; @ for price
        /// </summary>
        private void AddTooltip(GameObject objectToAdd, string leftUpCornerText, string description, string rightUpCornerText, bool isTooltipEnabled = false)
        {
            if (!objectToAdd.TryGetComponent(out SimpleTooltip tooltip))
            {
                tooltip = objectToAdd.AddComponent<SimpleTooltip>();
            }

            tooltip.isEnabled = isTooltipEnabled;

            if (tooltipStyle) tooltip.simpleTooltipStyle = tooltipStyle;
            tooltip.infoLeft += "~" + leftUpCornerText + "\n";
            tooltip.infoLeft += "^" + description;
            tooltip.infoRight += "@$" + rightUpCornerText;
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
                GameObject currentItem = AssembleItemIcons(currentDataElement.name, currentDataElement.icon, currentDataElement.itemSprite, currentDataElement.type, currentDataElement.unlockLevel, currentDataElement.price, currentDataElement.itemDescription, currentDataElement.lockedInformation);

                currentItem.transform.SetParent(consumableStore.transform);
                currentItem.transform.localScale = Vector3.one;
            }
        }
    }
}
