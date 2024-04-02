using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScratchCardGeneration.ScratchCardData
{
    [CreateAssetMenu(fileName = "ScratchCardPitySettings", menuName = "ScriptableObjects/ScratchCardPitySetting",
        order = 0)]
    public class ScratchCardPitySystemData : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "Brand", ValueLabel = "Pity Setting")]
        public Dictionary<ScratchCardBrand, PitySetting> dataList = new Dictionary<ScratchCardBrand, PitySetting>()
        {
            {
                ScratchCardBrand.Fruities, new PitySetting()
            }
        };

        [Serializable]
        public class PitySetting
        {
            public float costThreshold;
            public float winningProbabilityOverThreshold;
        }
    }
}