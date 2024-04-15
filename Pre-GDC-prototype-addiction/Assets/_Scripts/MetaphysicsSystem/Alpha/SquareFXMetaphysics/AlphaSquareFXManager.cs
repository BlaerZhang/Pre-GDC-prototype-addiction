using System;
using System.Collections.Generic;
using ScratchCardGeneration.LayoutConstructor;
using ScratchCardGeneration.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MetaphysicsSystem.Alpha.SquareFXMetaphysics
{
    public class AlphaSquareFXManager : SerializedMonoBehaviour
    {
        public enum FXType
        {
            Positive,
            Negative,
            None
        }

        public enum PrizeType
        {
            Big,
            Small,
            None
        }

        public GameObject lightEffectPrefab;
        public float prizeQualityThreshold;

        // 2: big prize, 1: small prize, 0: no prize
        public readonly Dictionary<PrizeType, Dictionary<FXType, float>> FXTypeProbabilityDict = new Dictionary<PrizeType, Dictionary<FXType, float>>();

        private List<AlphaSquareFX> currentAlphaSquareFxList;

        private void OnEnable()
        {
            FruitiesLayoutConstructor.onScratchCardConstructed += CheckPrizeType;
            AlphaSquareFX.onFXMoveEnd += DestroyOverlappedFX;
        }

        private void OnDisable()
        {
            FruitiesLayoutConstructor.onScratchCardConstructed -= CheckPrizeType;
            AlphaSquareFX.onFXMoveEnd -= DestroyOverlappedFX;
        }

        // called when generating the card
        private void CheckPrizeType(float totalPrize)
        {
            FXType fxType;
            if (totalPrize >= prizeQualityThreshold)
            {
                // randomly get a FX type
                fxType = Utils.CalculateMultiProbability(FXTypeProbabilityDict[PrizeType.Big]);
            }
            else if (totalPrize < prizeQualityThreshold || totalPrize > 0)
            {
                fxType = Utils.CalculateMultiProbability(FXTypeProbabilityDict[PrizeType.Small]);
            }
            else
            {
                fxType = Utils.CalculateMultiProbability(FXTypeProbabilityDict[PrizeType.None]);
            }
            SpawnLightEffectByType(fxType);
        }

        private void SpawnLightEffectByType(FXType fxType)
        {
            if (fxType.Equals(FXType.None)) return;

            GameObject fxObject = Instantiate(lightEffectPrefab);
            AlphaSquareFX alphaSquareFX = fxObject?.GetComponent<AlphaSquareFX>();
            if (alphaSquareFX) currentAlphaSquareFxList.Add(alphaSquareFX);

            switch (fxType)
            {
                case FXType.Positive:
                    if (alphaSquareFX) alphaSquareFX.isPositive = true;
                    break;
                case FXType.Negative:
                    if (alphaSquareFX) alphaSquareFX.isPositive = false;
                    break;
            }
        }

        private void DestroyOverlappedFX()
        {
            int fxCount = currentAlphaSquareFxList.Count;
            if (fxCount <= 1) return;

            for (int i = 0; i < currentAlphaSquareFxList.Count - 1; i++)
            {
                Vector2Int fxGrid = currentAlphaSquareFxList[i].currentGrid;
                for (int j = i + 1; j < currentAlphaSquareFxList.Count; j++)
                {
                    Vector2Int otherFxGrid = currentAlphaSquareFxList[j].currentGrid;

                    if (fxGrid.Equals(otherFxGrid))
                    {
                        Destroy(currentAlphaSquareFxList[j]);
                        currentAlphaSquareFxList.Remove(currentAlphaSquareFxList[j]);
                    }
                }
            }
        }
    }
}