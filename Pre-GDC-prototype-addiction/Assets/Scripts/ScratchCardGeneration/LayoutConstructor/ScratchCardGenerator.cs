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
        public Dictionary<ScratchCardBrand, ICardLayoutConstructor> cardLayoutConstructorDic;

        [HideInInspector] public GameObject currentScratchCard;

        private void OnEnable()
        {
            BuyCardManager.onScratchCardSelected += AssembleScratchCard;
        }

        private void OnDisable()
        {
            BuyCardManager.onScratchCardSelected -= AssembleScratchCard;
        }

        private void AssembleScratchCard(ScratchCardBrand currentCardBrand, int level, float price)
        {
            print("assenms bfhbv");
            var prizeDistribution = prizeDistributionData.dataList[currentCardBrand].levelDistribution[level];
            float costThreshold = prizePityData.dataList[currentCardBrand].costThreshold;
            float winningProbabilityOverThreshold = prizePityData.dataList[currentCardBrand].winningProbabilityOverThreshold;

            currentCardPrize = BasePrizeGenerator.GeneratePrize(prizeDistribution, costThreshold, winningProbabilityOverThreshold);
            print("prize generated");

            // record total gold spent before winning
            GameManager.Instance.totalCostBeforeWinning += price;
            Debug.Log($"totalCostBeforeWinning: {GameManager.Instance.totalCostBeforeWinning}");

            currentScratchCard = SwitchConstructor(currentCardBrand).ConstructCardLayout(currentCardPrize);
            print("SwitchConstructor");
        }

        private ICardLayoutConstructor SwitchConstructor(ScratchCardBrand currentCardBrand)
        {
            return cardLayoutConstructorDic[currentCardBrand];
        }
    }
}
