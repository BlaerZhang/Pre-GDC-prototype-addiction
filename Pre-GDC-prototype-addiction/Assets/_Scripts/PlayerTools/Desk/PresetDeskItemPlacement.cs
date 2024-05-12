using System.Collections.Generic;
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

    private void InitializeItem()
    {
        Transform deskItems = transform.Find("Desk Items");
        int itemCount = deskItems.childCount;

        for (int i = 0; i < itemCount; i++)
        {
            if (i == 0) continue;
            var item = deskItems.GetChild(i).gameObject;
            deskItemList.Add(item);
            item.SetActive(false);
        }
    }
}
