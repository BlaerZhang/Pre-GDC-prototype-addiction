using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace _Scripts.ConsumableStore
{
    [CreateAssetMenu(fileName = "ConsumableItemsData", menuName = "ScriptableObjects/ConsumableItemsData", order = 0)]
    public class ConsumableItemsData : ScriptableObject
    {
        [Serializable]
        public struct ItemInformation
        {
            public string name;
            [PreviewField(Alignment = ObjectFieldAlignment.Center)]
            [TableColumnWidth(57, Resizable = false)]
            public Sprite icon;
            [PreviewField(Alignment = ObjectFieldAlignment.Center)]
            public Sprite itemSprite;
            public ConsumableType type;
            public int unlockLevel;
            public int price;
            [TextArea]
            public string itemDescription;
            [TableColumnWidth(240, Resizable = false)]
            public LockedInformation lockedInformation;
        }

        [Serializable]
        public struct LockedInformation
        {
            public string rightCornerText;
            public string leftCornerText;
            [TextArea]
            public string lockedDescription;
        }

        [TableList(ShowIndexLabels = true)]
        public List<ItemInformation> consumableItemsDataList = new()
        {
            new ItemInformation(),
            new ItemInformation(),
        };
    }
}