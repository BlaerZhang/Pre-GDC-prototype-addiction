using System;
using System.Collections.Generic;
using _Scripts.ConsumableStore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.PlayerTools
{
    public class DeskItemPlacement : MonoBehaviour
    {
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("Please place position holder as this object's child to indicates the item slot position", UnityEditor.MessageType.Info);
        }

        [Serializable]
        public class ItemSlot
        {
            [HideInInspector] public GameObject itemHere;
            public Vector2 slotPosition;

            public ItemSlot(Vector2 slotPosition)
            {
                itemHere = null;
                this.slotPosition = slotPosition;
            }
        }

        private ItemSlot[] itemSlots;
        private int maxItems;

        private void Start()
        {
            InitializeItemSlots();
        }

        private void InitializeItemSlots()
        {
            maxItems = transform.childCount;
            itemSlots = new ItemSlot[maxItems];

            print(itemSlots.Length);

            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i] = new ItemSlot(transform.GetChild(i).position);
            }
        }

        private void OnEnable()
        {
            ConsumableItemIcon.onItemBought += PlaceItemOnDesk;
            ConsumableItemBase.onItemRemoved += RemoveItemFromDesk;
        }

        private void OnDisable()
        {
            ConsumableItemIcon.onItemBought -= PlaceItemOnDesk;
            ConsumableItemBase.onItemRemoved -= RemoveItemFromDesk;
        }

        private void PlaceItemOnDesk(GameObject newItem)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].itemHere == null)
                {
                    var item = Instantiate(newItem, itemSlots[i].slotPosition, Quaternion.identity);
                    item.name = newItem.name;
                    item.transform.SetParent(transform);

                    itemSlots[i].itemHere = item;
                    Debug.Log("Item added at position: " + i);

                    return;
                }
            }

            // TODO: add desk item capacity limit
            // Debug.Log("No space available to add the item.");
        }

        private void RemoveItemFromDesk(string itemName)
        {
            foreach (var slot in itemSlots)
            {
                var itemHere = slot.itemHere;
                if (itemHere == null) continue;
                if (itemHere.name.Equals(itemName))
                {
                    print($"{name} is removed from desk");
                    // TODO: remove item effects
                    Destroy(itemHere);
                    slot.itemHere = null;
                }
            }
        }
    }
}