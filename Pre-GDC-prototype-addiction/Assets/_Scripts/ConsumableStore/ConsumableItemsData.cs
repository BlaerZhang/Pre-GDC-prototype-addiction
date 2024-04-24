using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace _Scripts.ConsumableStore
{
    [CreateAssetMenu(fileName = "ConsumableItemsData", menuName = "ScriptableObjects/ConsumableItemsData", order = 0)]
    public class ConsumableItemsData : ScriptableObject
    {
        [Serializable]
        public struct ItemInformation
        {
            public string name;
            [TableColumnWidth(57, Resizable = false)]
            [PreviewField(Alignment = ObjectFieldAlignment.Center)]
            public Sprite icon;
            public ConsumableType type;
            [TableColumnWidth(80, Resizable = false)]
            public int unlockLevel;
            public float price;
            [TextArea]
            public string description;
        }

        [TableList(ShowIndexLabels = true)]
        public List<ItemInformation> consumableItemsDataList = new()
        {
            new ItemInformation(),
            new ItemInformation(),
        };
    }
}