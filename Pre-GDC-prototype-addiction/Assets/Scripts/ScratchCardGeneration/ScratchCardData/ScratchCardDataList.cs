using System;
using System.Collections.Generic;
using ScratchCardGeneration.PrizeGenerator;
using UnityEngine;
using Sirenix.OdinInspector;

namespace ScratchCardGeneration.LayoutConstructor
{
    [CreateAssetMenu(fileName = "ScratchCardPitySettings", menuName = "ScriptableObjects/ScratchCardPitySetting",
        order = 0)]
    public class ScratchCardPitySystemData : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "Brand", ValueLabel = "Level Pity Setting")]
        public Dictionary<ScratchCardBrand, LevelPitySetting> dataList = new Dictionary<ScratchCardBrand, LevelPitySetting>()
        {
            {
                ScratchCardBrand.Fruities, new LevelPitySetting()
            }
        };

        [Serializable]
        public class LevelPitySetting
        {
            [DictionaryDrawerSettings(KeyLabel = "Level", ValueLabel = "Pity Setting")]
            public Dictionary<int, PitySetting> levelPitySetting;
        }

        [Serializable]
        public class PitySetting
        {
            public float costThreshold;
            public float winningProbabilityOverThreshold;
        }
    }
}