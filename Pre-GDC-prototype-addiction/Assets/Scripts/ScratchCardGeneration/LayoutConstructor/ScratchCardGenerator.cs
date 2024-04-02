using System;
using System.Collections.Generic;
using System.Linq;
using ScratchCardGeneration.PrizeGenerator;
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

        private void OnEnable()
        {
            // onScratchCardSelected += AssembleScratchCard;
        }

        private void OnDisable()
        {
            // onScratchCardSelected -= AssembleScratchCard;
        }

        private void AssembleScratchCard(ScratchCardBrand currentCardBrand, int level, float price)
        {
            var prizeDistribution = prizeDistributionData.dataList[currentCardBrand].levelDistribution[level];
            float costThreshold = prizePityData.dataList[currentCardBrand].costThreshold;
            float winningProbabilityOverThreshold = prizePityData.dataList[currentCardBrand].winningProbabilityOverThreshold;

            currentCardPrize = BasePrizeGenerator.GeneratePrize(prizeDistribution, costThreshold, winningProbabilityOverThreshold);

            // record total gold spent before winning
            GameManager.Instance.totalCostBeforeWinning += price;
            Debug.Log($"totalCostBeforeWinning: {GameManager.Instance.totalCostBeforeWinning}");

            SwitchConstructor(currentCardBrand).ConstructCardLayout(currentCardPrize);
        }

        private ICardLayoutConstructor SwitchConstructor(ScratchCardBrand currentCardBrand)
        {
            return cardLayoutConstructorDic[currentCardBrand];
        }
    }
}
