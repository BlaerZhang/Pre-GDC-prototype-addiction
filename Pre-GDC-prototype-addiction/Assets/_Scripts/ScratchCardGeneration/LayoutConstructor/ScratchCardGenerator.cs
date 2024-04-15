using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using ScratchCardGeneration.PrizeGenerator;
using ScratchCardGeneration.ScratchCardData;
using ScratchCardGeneration.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ScratchCardGeneration.LayoutConstructor
{
    public class ScratchCardGenerator : SerializedMonoBehaviour
    {
        [Header("Prize Setting")]
        [HideInInspector] public float currentCardPrize;

        public ScratchCardPrizeDistributionData prizeDistributionData;
        public ScratchCardPitySystemData prizePityData;

        [Header("Layout Setting")]
        public Dictionary<ScratchCardBrand, ICardLayoutConstructor> CardLayoutConstructorDic = new();

        [HideInInspector] public GameObject currentScratchCard;

        private void OnEnable()
        {
            BuyCardManager.onScratchCardSelected += AssembleScratchCard;
        }

        private void OnDisable()
        {
            BuyCardManager.onScratchCardSelected -= AssembleScratchCard;
        }

        private void AssembleScratchCard(ScratchCardBrand currentCardBrand, int level, float price, Vector3 generatePos)
        {
            print($"level: {level}");

            var prizeDistributionDataList = prizeDistributionData.dataList[currentCardBrand];
            var pityDataList = prizePityData.dataList[currentCardBrand];

            var prizeDistribution = prizeDistributionDataList.levelDistribution[level];
            float costThreshold = pityDataList.levelPitySetting[level].costThreshold;
            float winningProbabilityOverThreshold = prizePityData.dataList[currentCardBrand].levelPitySetting[level].winningProbabilityOverThreshold;

            currentCardPrize = BasePrizeGenerator.GeneratePrize(prizeDistribution, costThreshold, winningProbabilityOverThreshold);

            // record total gold spent before winning
            GameManager.Instance.totalCostBeforeWinning += price;
            Debug.Log($"totalCostBeforeWinning: {GameManager.Instance.totalCostBeforeWinning}");

            currentScratchCard = SwitchConstructor(currentCardBrand).ConstructCardLayout(currentCardPrize, price, generatePos);
        }

        private ICardLayoutConstructor SwitchConstructor(ScratchCardBrand currentCardBrand)
        {
            return CardLayoutConstructorDic[currentCardBrand];
        }
    }
}
