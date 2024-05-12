using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.ScratchCardGeneration.ScratchCardData
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
            public Dictionary<int, PitySetting> levelPitySetting = new Dictionary<int, PitySetting>();
        }

        [Serializable]
        public class PitySetting
        {
            public float costThreshold;
            public float winningProbabilityOverThreshold;
        }
    }
}