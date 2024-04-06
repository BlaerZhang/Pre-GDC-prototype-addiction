using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScratchCardGeneration.ScratchCardData
{
    [CreateAssetMenu(fileName = "ScratchCardPrizeDistributionData", menuName = "ScriptableObjects/ScratchCardPrizeDistribution", order = 0)]
    public class ScratchCardPrizeDistributionData : SerializedScriptableObject
    {

        [Serializable]
        public class Distribution
        {
            [DictionaryDrawerSettings(KeyLabel = "Level", ValueLabel = "Distribution")]
            public Dictionary<int, Dictionary<int, float>> levelDistribution;
        }

        [DictionaryDrawerSettings(KeyLabel = "Brand", ValueLabel = "Level And Distribution")]
        public Dictionary<ScratchCardBrand, Distribution> dataList = new Dictionary<ScratchCardBrand, Distribution>()
        {
            {
                ScratchCardBrand.Fruities, new Distribution()
            }
        };
    }
}