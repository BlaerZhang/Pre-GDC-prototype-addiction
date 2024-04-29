using System.Collections;
using System.Collections.Generic;
using _Scripts.ConsumableStore;
using Sirenix.OdinInspector;
using UnityEngine;

public class PresetDeskItemPlacement : MonoBehaviour
{
    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        UnityEditor.EditorGUILayout.HelpBox("Set items as this object's child", UnityEditor.MessageType.Info);
    }

    private struct FixedItemSlot
    {
        [HideInInspector] public GameObject itemHere;
        public Vector2 slotPosition;

        public FixedItemSlot(Vector2 slotPosition)
        {
            itemHere = null;
            this.slotPosition = slotPosition;
        }
    }

    private FixedItemSlot[] fixedItemSlots;
    private int maxItems;

    private FixedItemSlot firstAvailableSlot;

    public static bool isDeskFull = false;

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

    private void InitializeItemSlots()
    {

    }

    private void CheckFirstAvailableSlot(GameObject newItem)
    {
        int slotNumber = fixedItemSlots.Length;
        for (int i = 0; i < slotNumber; i++)
        {
            if (fixedItemSlots[i].itemHere == null)
            {
                firstAvailableSlot = fixedItemSlots[i];
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
        foreach (var slot in fixedItemSlots)
        {
            var itemHere = slot.itemHere;
            if (itemHere == null) continue;
            if (itemHere.name.Equals(itemName))
            {
                print($"{name} is removed from desk");
                // TODO: remove item effects
                slot.itemHere.SetActive(false);

                if (isDeskFull) isDeskFull = false;
            }
        }
    }
}
