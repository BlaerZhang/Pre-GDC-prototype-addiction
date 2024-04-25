using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.ConsumableStore
{
    public class ConsumableStoreInitializer : MonoBehaviour
    {
        [Title("Consumable Item Data")]
        [SerializeField] private ConsumableItemsData consumableItemsData;

        [Title("Item Placement")]
        [SerializeField] private Sprite lockedIcon;

        [SerializeField] private Vector2 startPosition;
        [SerializeField] private float cellSize;
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private float gapLengthX;
        [SerializeField] private float gapLengthY;

        [Title("Item Tooltip Settings")]
        [SerializeField] private SimpleTooltipStyle tooltipStyle;


        void Start() => PlaceItems();

        /// <summary>
        /// assemble items from the data list
        /// </summary>
        private GameObject AssembleItems(string itemName, Sprite icon, ConsumableType consumableType, int unlockLevel, float price, string description)
        {
            GameObject consumableItem = new GameObject(itemName);
            consumableItem.AddComponent<SpriteRenderer>().sprite = icon;
            consumableItem.AddComponent<ConsumableItem>().InitializeItem(itemName, consumableType, unlockLevel, price, description);
            var tooltip = consumableItem.AddComponent<SimpleTooltip>();

            // TODO: put name, icon, price, description into the tooltip
            tooltip.simpleTooltipStyle = tooltipStyle;

            return consumableItem;
        }

        /// <summary>
        /// place the items on the shelf
        /// </summary>
        private void PlaceItems()
        {
            GameObject consumableStore = new GameObject("ConsumableStore")
            {
                transform =
                {
                    position = Vector2.zero
                }
            };

            int row = gridSize.x;
            int col = gridSize.y;

            Vector2 topLeftStartPosition = new Vector2(startPosition.x, startPosition.y + (row - 1) * (cellSize + gapLengthY));

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    int itemIndex = i * col + j;

                    var currentDataElement = consumableItemsData.consumableItemsDataList[itemIndex];
                    GameObject currentItem = AssembleItems(currentDataElement.name, currentDataElement.icon, currentDataElement.type, currentDataElement.unlockLevel, currentDataElement.price, currentDataElement.description);

                    Vector2 cellPosition = topLeftStartPosition + new Vector2(j * (cellSize + gapLengthX), -i * (cellSize + gapLengthY));
                    currentItem.transform.SetParent(consumableStore.transform);
                    currentItem.transform.position = cellPosition;

                    if (itemIndex == consumableItemsData.consumableItemsDataList.Count - 1) return;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    Vector2 cellPosition = new Vector2(j * (cellSize + gapLengthX), i * (cellSize + gapLengthY)) + startPosition;
                    Gizmos.DrawWireCube(cellPosition, cellSize * Vector2.one);
                }
            }
        }
    }
}
