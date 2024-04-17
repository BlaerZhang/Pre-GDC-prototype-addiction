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

        private void AssembleScratchCard(ScratchCardBrand currentCardBrand, int level, float price, Vector3 generatePosition, Sprite cardFace)
        {
            print($"level: {level}");

            var prizeDistributionDataList = prizeDistributionData.dataList[currentCardBrand];
            var pityDataList = prizePityData.dataList[currentCardBrand];
            
            var prizeDistribution = prizeDistributionDataList.levelDistribution[level];
            float costThreshold = level < pityDataList.levelPitySetting.Count ? pityDataList.levelPitySetting[level].costThreshold : 0;
            float winningProbabilityOverThreshold = level < pityDataList.levelPitySetting.Count ? pityDataList.levelPitySetting[level].winningProbabilityOverThreshold : 0;

            currentCardPrize = BasePrizeGenerator.GeneratePrize(prizeDistribution, costThreshold, winningProbabilityOverThreshold);

            // record total gold spent before winning
            // GameManager.Instance.totalCostBeforeWinning += price;
            // Debug.Log($"totalCostBeforeWinning: {GameManager.Instance.totalCostBeforeWinning}");

            currentScratchCard = SwitchConstructor(currentCardBrand).ConstructCardLayout(currentCardPrize, price, generatePosition);

            GenerateCardFace(cardFace);
        }

        private ICardLayoutConstructor SwitchConstructor(ScratchCardBrand currentCardBrand)
        {
            return CardLayoutConstructorDic[currentCardBrand];
        }

        [Header("Scratch Field Setting")]
        public GameObject scratchBackgroundPrefab;

        private void GenerateCardFace(Sprite cardFace)
        {
            if (!scratchBackgroundPrefab)
            {
                Debug.LogError("scratchBackgroundPrefab is null!");
                return;
            }

            GameObject scratchBackground = Instantiate(scratchBackgroundPrefab, Vector3.zero, Quaternion.identity);
            scratchBackground.transform.SetParent(currentScratchCard.transform);
            scratchBackground.transform.localPosition = Vector2.zero;

            if (scratchBackground.TryGetComponent(out SpriteRenderer spriteRenderer)) spriteRenderer.sprite = cardFace;
        }
    }
}
