using System;
using System.Collections.Generic;
using _Scripts.ConsumableStore;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.PlayerTools
{
    public class DeskItemPlacement : MonoBehaviour
    {
#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("Please place position holder as this object's child to indicates the item slot position", UnityEditor.MessageType.Info);
        }
#endif

        [Serializable]
        private class ItemSlot
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

        private ItemSlot firstAvailableSlot;

        public static bool isDeskFull = false;

        private void Start()
        {
            InitializeItemSlots();
        }

        private void InitializeItemSlots()
        {
            maxItems = transform.childCount;
            itemSlots = new ItemSlot[maxItems];

            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i] = new ItemSlot(transform.GetChild(i).position);
            }
        }

        private void OnEnable()
        {
            ConsumableItemIcon.onTryBuyItem += CheckFirstAvailableSlot;
            ConsumableItemBase.onItemRemoved += RemoveItemFromDesk;
        }

        private void OnDisable()
        {
            ConsumableItemIcon.onTryBuyItem -= CheckFirstAvailableSlot;
            ConsumableItemBase.onItemRemoved -= RemoveItemFromDesk;
        }

        private void CheckFirstAvailableSlot(GameObject newItem)
        {
            int slotNumber = itemSlots.Length;
            for (int i = 0; i < slotNumber; i++)
            {
                if (itemSlots[i].itemHere == null)
                {
                    firstAvailableSlot = itemSlots[i];
                    PlaceItemOnDesk(newItem);
                    return;
                }
            }

            // if no available space on desk
            isDeskFull = true;
            Debug.Log("No space available on desk to add the item.");
        }

        private void PlaceItemOnDesk(GameObject newItem)
        {
            var item = Instantiate(newItem, firstAvailableSlot.slotPosition, Quaternion.identity);
            item.name = newItem.name;
            item.transform.SetParent(transform);

            firstAvailableSlot.itemHere = item;
            Debug.Log("Item added at position: " + firstAvailableSlot.slotPosition);
        }

        private void RemoveItemFromDesk(string itemName)
        {
            foreach (var slot in itemSlots)
            {
                var itemHere = slot.itemHere;
                if (itemHere == null) continue;
                if (itemHere.name.Equals(itemName))
                {
                    print($"{itemName} is removed from desk");
                    // TODO: remove item effects
                    Destroy(itemHere);
                    slot.itemHere = null;

                    if (isDeskFull) isDeskFull = false;
                }
            }
        }
    }
}