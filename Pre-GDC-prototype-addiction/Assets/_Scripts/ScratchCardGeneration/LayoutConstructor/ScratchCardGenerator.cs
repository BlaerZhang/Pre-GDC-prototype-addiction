using System.Collections.Generic;
using _Scripts.Interaction.PosterPicking;
using _Scripts.ScratchCardGeneration.PrizeGenerator;
using _Scripts.ScratchCardGeneration.ScratchCardData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.ScratchCardGeneration.LayoutConstructor
{
    public class ScratchCardGenerator : SerializedMonoBehaviour
    {
        [Title("Prize Setting")]
        [HideInInspector] public float currentCardPrize;

        [SerializeField] public ScratchCardPrizeDistributionData prizeDistributionData;
        [SerializeField] public ScratchCardPitySystemData prizePityData;

        [Title("Layout Setting")]
        [SerializeField] public Dictionary<ScratchCardBrand, ICardLayoutConstructor> CardLayoutConstructorDic = new();
        [SerializeField] private Vector2 cardGenerationPosition;

        [HideInInspector] public GameObject currentScratchCard;

        private void OnEnable()
        {
            ScratchCardDealer.onToScratchStage += AssembleScratchCard;
        }

        private void OnDisable()
        {
            ScratchCardDealer.onToScratchStage -= AssembleScratchCard;
        }

        private void AssembleScratchCard(ScratchCardBrand currentCardBrand, int level, int originalPrice, Sprite cardFace)
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

            currentScratchCard = SwitchConstructor(currentCardBrand).ConstructCardLayout(currentCardPrize, originalPrice, cardGenerationPosition);
            var sortingGroup = currentScratchCard.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Scratch Card";
            sortingGroup.sortingOrder = 1;

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
            print("cardFace " + cardFace);
            if (!scratchBackgroundPrefab)
            {
                Debug.LogError("scratchBackgroundPrefab is null!");
                return;
            }

            if (!cardFace)
            {
                Debug.LogError("cardFace is null!");
                return;
            }

            GameObject scratchBackground = Instantiate(scratchBackgroundPrefab, Vector3.zero, Quaternion.identity);
            scratchBackground.transform.SetParent(currentScratchCard.transform);
            scratchBackground.transform.localPosition = Vector2.zero;

            if (scratchBackground.TryGetComponent(out SpriteRenderer spriteRenderer)) spriteRenderer.sprite = cardFace;
        }
    }
}
