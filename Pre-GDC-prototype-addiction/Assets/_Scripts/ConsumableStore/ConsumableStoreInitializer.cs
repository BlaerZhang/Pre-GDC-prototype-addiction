using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.ConsumableStore
{
    // TODO: hide/show store
    public class ConsumableStoreInitializer : MonoBehaviour
    {
        [Title("Consumable Item Data")]
        [SerializeField] private ConsumableItemsData consumableItemsData;

        [Title("Item Placement")]
        [SerializeField] private GridLayoutGroup consumableStore;

        [SerializeField] private Transform startPosition;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private Vector2 gapLength;
        [SerializeField] private int gridLayoutSizeConstraint;

        // [SerializeField] private Sprite lockedIcon;

        [Title("Item Tooltip Settings")]
        [SerializeField] private SimpleTooltipStyle tooltipStyle;

        void Start()
        {
            SetUpGridLayout();
            PlaceItems();
        }

        private void SetUpGridLayout()
        {
            consumableStore.transform.position = startPosition.position;

            consumableStore.startCorner = GridLayoutGroup.Corner.UpperLeft;
            consumableStore.startAxis = GridLayoutGroup.Axis.Vertical;
            consumableStore.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            consumableStore.constraintCount = gridLayoutSizeConstraint;

            consumableStore.cellSize = cellSize;
            consumableStore.spacing = gapLength;

            // consumableStore.cellLayout
        }

        /// <summary>
        /// assemble items from the data list
        /// </summary>
        private GameObject AssembleItems(string itemName, Sprite icon, ConsumableType consumableType, int unlockLevel, float price, string description)
        {
            GameObject consumableItem = new GameObject(itemName);
            var image = consumableItem.AddComponent<Image>();
            image.sprite = icon;
            image.color = Color.black;

            var tooltip = consumableItem.AddComponent<SimpleTooltip>();
            tooltip.isEnabled = false;

            // TODO: put name, icon, price, description into the tooltip
            if (tooltipStyle) tooltip.simpleTooltipStyle = tooltipStyle;

            consumableItem.AddComponent<ConsumableItem>().InitializeItem(itemName, consumableType, unlockLevel, price, description);

            return consumableItem;
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
                GameObject currentItem = AssembleItems(currentDataElement.name, currentDataElement.icon, currentDataElement.type, currentDataElement.unlockLevel, currentDataElement.price, currentDataElement.description);

                currentItem.transform.SetParent(consumableStore.transform);
                currentItem.transform.localScale = Vector3.one;
            }
        }
    }
}
