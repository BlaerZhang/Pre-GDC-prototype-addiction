using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.ConsumableStore;
using Sirenix.OdinInspector;
using UnityEngine;

public class PresetDeskItemPlacement : MonoBehaviour
{
#if UNITY_EDITOR
    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        UnityEditor.EditorGUILayout.HelpBox("Set slot as this object's child, slot must have the same name with the item", UnityEditor.MessageType.Info);
    }
#endif

    // item name / item
    private List<GameObject> deskItemList = new();

    private void Start()
    {
        InitializeItem();
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

    private void InitializeItem()
    {
        int itemCount = transform.childCount;

        if (itemCount == 0) return;

        for (int i = 0; i < itemCount; i++)
        {
            if (i == 0) continue;
            var item = transform.GetChild(i).gameObject;
            deskItemList.Add(item);
            item.SetActive(false);
        }
    }

    private void CheckFirstAvailableSlot(GameObject newItem)
    {
        foreach (var item in deskItemList)
        {
            if (item.name.Equals(newItem.name))
            {
                item.SetActive(true);
                return;
            }
        }

        Debug.LogError("No preset item found on desk when adding");
    }

    private void RemoveItemFromDesk(string itemName)
    {
        foreach (var item in deskItemList)
        {
            if (item.name.Equals(itemName))
            {
                item.SetActive(false);
                return;
            }
        }

        Debug.LogError("No preset item found on desk when removing");
    }
}
