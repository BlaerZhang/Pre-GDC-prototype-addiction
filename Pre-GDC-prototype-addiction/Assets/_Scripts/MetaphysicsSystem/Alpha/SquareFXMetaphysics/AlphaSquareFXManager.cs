using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Interaction;
using Manager;
using ScratchCardGeneration;
using ScratchCardGeneration.LayoutConstructor;
using ScratchCardGeneration.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MetaphysicsSystem.Alpha.SquareFXMetaphysics
{
    public class AlphaSquareFXManager : SerializedMonoBehaviour
    {
        public enum PrizeType
        {
            Big,
            Small,
            None
        }

        public enum FXType
        {
            Positive,
            Negative,
            None
        }

        public enum FXActionName
        {
            Split,
            Disappear,
            Stay,
            Move
        }

        public GameObject positiveLightEffectPrefab;
        public GameObject negativeLightEffectPrefab;

        private float prizeTypeThreshold;

        [Header("Price Threshold Generation")]
        [DictionaryDrawerSettings(KeyLabel = "Card Price", ValueLabel = "Price Type Threshold")]
        public readonly Dictionary<float, float> PrizeTypeThresholdDict = new()
        {
            { 100, 200 },
            { 200, 200 },
            { 500, 200 },
        };

        [Header("FX Type Generation")]
        [DictionaryDrawerSettings(KeyLabel = "Prize Type", ValueLabel = "FX Type Distribution")]
        public readonly Dictionary<PrizeType, Dictionary<FXType, float>> FXTypeProbabilityDict = new()
        {
            { PrizeType.Big, new()
            {
                { FXType.Positive, 0 },
                { FXType.Negative, 0 },
                { FXType.None, 0 },
            } },
            { PrizeType.Small, new()
            {
                { FXType.Positive, 0 },
                { FXType.Negative, 0 },
                { FXType.None, 0 },
            }},
            { PrizeType.None, new()
            {
                { FXType.Positive, 0 },
                { FXType.Negative, 0 },
                { FXType.None, 0 },
            }}
        };

        [Header("Positive Effect")]
        [Header("Scratch no FX and FX not on winning grid")]
        [DictionaryDrawerSettings(KeyLabel = "Action Name", ValueLabel = "Probability")]
        public Dictionary<FXActionName, float> PositiveScratchNoFXAndFXNotOnWinningGridProbabilityList;
        [Header("Scratch no FX but FX on winning grid")]
        [DictionaryDrawerSettings(KeyLabel = "Action Name", ValueLabel = "Probability")]
        public Dictionary<FXActionName, float> PositiveScratchNoFXButFXOnWinningGridProbabilityList;
        [Header("Scratch FX but FX not on winning grid")]
        [DictionaryDrawerSettings(KeyLabel = "Action Name", ValueLabel = "Probability")]
        public Dictionary<FXActionName, float> PositiveScratchFXButFXNotOnWinningGridProbabilityList;
        [Header("Scratch FX and FX on winning grid")]
        [DictionaryDrawerSettings(KeyLabel = "Action Name", ValueLabel = "Probability")]
        public Dictionary<FXActionName, float> PositiveScratchFXAndFXOnWinningGridProbabilityList;

        [Header("Negative Effect")]
        [Header("Scratch no FX and FX not on winning grid")]
        [DictionaryDrawerSettings(KeyLabel = "Action Name", ValueLabel = "Probability")]
        public Dictionary<FXActionName, float> NegativeScratchNoFXAndFXNotOnWinningGridProbabilityList;
        [Header("Scratch no FX but FX on winning grid")]
        [DictionaryDrawerSettings(KeyLabel = "Action Name", ValueLabel = "Probability")]
        public Dictionary<FXActionName, float> NegativeScratchNoFXButFXOnWinningGridProbabilityList;
        [Header("Scratch FX but FX not on winning grid")]
        [DictionaryDrawerSettings(KeyLabel = "Action Name", ValueLabel = "Probability")]
        public Dictionary<FXActionName, float> NegativeScratchFXButFXNotOnWinningGridProbabilityList;
        [Header("Scratch FX and FX on winning grid")]
        [DictionaryDrawerSettings(KeyLabel = "Action Name", ValueLabel = "Probability")]
        public Dictionary<FXActionName, float> NegativeScratchFXAndFXOnWinningGridProbabilityList;

        [Header("FX Action Settings")]
        public float moveDuration = 0.1f;
        private bool isMoving = false;

        // prize grid
        private FruitiesLayoutConstructor fruitiesLayoutConstructor;
        private Vector2Int gridSize;
        // if the grid is true, means it has been fully scratched
        private VariableMatrix<bool> scratchingStatusMatrix;
        private List<Vector2Int> prizeWinningGridList;
        private VariableMatrix<Vector2> gridPositionMatrix;

        private List<AlphaSquareFX> currentAlphaSquareFxList = new();
        private int fxCounter = 0;

        private void OnEnable()
        {
            PrizeRevealing.onFullyScratched += TriggerFXAction;
            FruitiesLayoutConstructor.onScratchCardConstructed += CheckPrizeType;
        }

        private void OnDisable()
        {
            PrizeRevealing.onFullyScratched -= TriggerFXAction;
            FruitiesLayoutConstructor.onScratchCardConstructed -= CheckPrizeType;
        }

        #region Data Creation
        [ContextMenu(nameof(CreateAllData))]
        private void CreateAllData()
        {
            CreateData(ref PositiveScratchNoFXAndFXNotOnWinningGridProbabilityList);
            CreateData(ref PositiveScratchNoFXButFXOnWinningGridProbabilityList);
            CreateData(ref PositiveScratchFXButFXNotOnWinningGridProbabilityList);
            CreateData(ref PositiveScratchFXAndFXOnWinningGridProbabilityList);

            CreateData(ref NegativeScratchNoFXAndFXNotOnWinningGridProbabilityList);
            CreateData(ref NegativeScratchNoFXButFXOnWinningGridProbabilityList);
            CreateData(ref NegativeScratchFXButFXNotOnWinningGridProbabilityList);
            CreateData(ref NegativeScratchFXAndFXOnWinningGridProbabilityList);
        }

        private void CreateData(ref Dictionary<FXActionName, float> dictionary)
        {
            dictionary = new()
            {
                { FXActionName.Split, 0 },
                { FXActionName.Disappear, 0 },
                { FXActionName.Move, 0 },
                { FXActionName.Stay, 0 },
            };
        }

        private void FetchDataFromConstructor()
        {
            fruitiesLayoutConstructor = (FruitiesLayoutConstructor)GameManager.Instance.scratchCardGenerator.CardLayoutConstructorDic[ScratchCardBrand.Fruities];
            prizeWinningGridList = fruitiesLayoutConstructor.prizeWinningGridList;
            gridPositionMatrix = fruitiesLayoutConstructor.PrizeCellPositionMatrix;
            scratchingStatusMatrix = fruitiesLayoutConstructor.ScratchingStatusMatrix;
            gridSize = fruitiesLayoutConstructor.prizeAreaGridSize;
        }
        #endregion

        #region FX Generation
        // called when generating the card
        private void CheckPrizeType(float totalPrize, float price)
        {
            FetchDataFromConstructor();

            SwitchPrizeTypeThreshold(price);
            print($"price: {price}");
            print($"totalPrize: {totalPrize}");

            FXType fxType;
            if (totalPrize >= prizeTypeThreshold)
            {
                // randomly get a FX type
                fxType = Utils.CalculateMultiProbability(FXTypeProbabilityDict[PrizeType.Big]);
            }
            else if (totalPrize < prizeTypeThreshold & totalPrize > 0)
            {
                fxType = Utils.CalculateMultiProbability(FXTypeProbabilityDict[PrizeType.Small]);
            }
            else
            {
                fxType = Utils.CalculateMultiProbability(FXTypeProbabilityDict[PrizeType.None]);
            }

            print($"fxType: {fxType}");
            // for (int i = 0; i < 2; i++)
            // {
                SpawnLightEffectByType(fxType);
            // }

        }

        private void SwitchPrizeTypeThreshold(float price)
        {
            prizeTypeThreshold = PrizeTypeThresholdDict[price];
            print($"prizeTypeThreshold: {prizeTypeThreshold}");
        }

        private void SpawnLightEffectByType(FXType fxType)
        {
            if (fxType.Equals(FXType.None)) return;

            GameObject fxObject = fxType.Equals(FXType.Positive) ? Instantiate(positiveLightEffectPrefab) : Instantiate(negativeLightEffectPrefab);
            AlphaSquareFX alphaSquareFX = fxObject?.GetComponent<AlphaSquareFX>();

            if (!alphaSquareFX)
            {
                Debug.LogError("No AlphaSquareFX Component Attached");
                return;
            }

            alphaSquareFX.transform.SetParent(GameObject.Find("currentScratchCard").transform);
            alphaSquareFX.currentGrid = Utils.SelectRandomGridFromMatrix(gridSize.x, gridSize.y);
            alphaSquareFX.transform.localPosition = gridPositionMatrix.GetElement(alphaSquareFX.currentGrid);
            print("current grid: " + alphaSquareFX.currentGrid);

            currentAlphaSquareFxList.Add(alphaSquareFX);

            if  (fxType.Equals(FXType.Positive))
            {
                alphaSquareFX.isPositive = true;
            }
            else if (fxType.Equals(FXType.Negative))
            {
                alphaSquareFX.isPositive = false;
            }
        }
        #endregion

        #region FX Function

        // triggered when fully scratched
        private void TriggerFXAction(Vector2Int currentFullyScratchedGrid)
        {
            if (isMoving) return;

            if (currentAlphaSquareFxList.Count == 0) return;

            var copyFxList = Utils.DeepCopyList(currentAlphaSquareFxList);
            fxCounter = 0;

            foreach (var fx in copyFxList)
            {
                SwitchActionProbabilityList(fx, currentFullyScratchedGrid);
                fxCounter++;
            }

            // StartCoroutine(DelayedOverlappingCheck(moveDuration));
        }

        IEnumerator DelayedOverlappingCheck(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            DestroyOverlappedFX();
        }

        private void SwitchActionProbabilityList(AlphaSquareFX currentFX, Vector2Int currentFullyScratchedGrid)
        {
            foreach (var p in prizeWinningGridList)
            {
                print(p);
            }

            if (currentFX.isPositive)
            {
                if (currentFX.currentGrid.Equals(currentFullyScratchedGrid))
                {
                    print(prizeWinningGridList.Contains(currentFX.currentGrid)
                        ? nameof(PositiveScratchFXAndFXOnWinningGridProbabilityList)
                        : nameof(PositiveScratchFXButFXNotOnWinningGridProbabilityList));

                    ApplyProbabilityList(prizeWinningGridList.Contains(currentFX.currentGrid)
                        ? PositiveScratchFXAndFXOnWinningGridProbabilityList
                        : PositiveScratchFXButFXNotOnWinningGridProbabilityList, currentFX);
                }
                else
                {
                    print(prizeWinningGridList.Contains(currentFX.currentGrid)
                        ? nameof(PositiveScratchNoFXButFXOnWinningGridProbabilityList)
                        : nameof(PositiveScratchNoFXAndFXNotOnWinningGridProbabilityList));

                    ApplyProbabilityList(prizeWinningGridList.Contains(currentFX.currentGrid)
                        ? PositiveScratchNoFXButFXOnWinningGridProbabilityList
                        : PositiveScratchNoFXAndFXNotOnWinningGridProbabilityList, currentFX);
                }
            }
            else
            {
                if (currentFX.currentGrid.Equals(currentFullyScratchedGrid))
                {
                    print(prizeWinningGridList.Contains(currentFX.currentGrid)
                        ? nameof(NegativeScratchFXAndFXOnWinningGridProbabilityList)
                        : nameof(NegativeScratchFXButFXNotOnWinningGridProbabilityList));

                    ApplyProbabilityList(prizeWinningGridList.Contains(currentFX.currentGrid)
                        ? NegativeScratchFXAndFXOnWinningGridProbabilityList
                        : NegativeScratchFXButFXNotOnWinningGridProbabilityList, currentFX);
                }
                else
                {
                    print(prizeWinningGridList.Contains(currentFX.currentGrid)
                        ? nameof(NegativeScratchNoFXButFXOnWinningGridProbabilityList)
                        : nameof(NegativeScratchNoFXAndFXNotOnWinningGridProbabilityList));

                    ApplyProbabilityList(prizeWinningGridList.Contains(currentFX.currentGrid)
                        ? NegativeScratchNoFXButFXOnWinningGridProbabilityList
                        : NegativeScratchNoFXAndFXNotOnWinningGridProbabilityList, currentFX);
                }
            }
        }

        private void ApplyProbabilityList(Dictionary<FXActionName, float> dict, AlphaSquareFX currentFX)
        {
            FXActionName actionName = Utils.CalculateMultiProbability(dict);
            print($"actionName: {actionName}");

            switch (actionName)
            {
                case FXActionName.Disappear:
                    Destroy(currentFX.gameObject);
                    currentAlphaSquareFxList.Remove(currentFX);
                    break;
                case FXActionName.Split:
                    SplitSelf(currentFX);
                    break;
                case FXActionName.Stay:
                    break;
                case FXActionName.Move:
                    MoveFXRandomly(currentFX);
                    break;
            }
        }

        private void MoveFXRandomly(AlphaSquareFX currentFX)
        {
            var notFullyScratchedGridList = scratchingStatusMatrix.GetIndexOfElement(false);

            if (notFullyScratchedGridList.Count == 0) return;

            Vector2Int newGrid = Utils.GetRandomElementFromList(notFullyScratchedGridList);

            Vector2 newWorldPosition = gridPositionMatrix.GetElement(newGrid) + (Vector2)currentFX.transform.parent.position;

            MoveFX(currentFX, newWorldPosition, newGrid);
        }

        /// <summary>
        /// spawn a new fx to a nearby not fully scratched grid
        /// </summary>
        private void SplitSelf(AlphaSquareFX currentFX)
        {
            // get nearby grids that are not fully scratched
            List<Vector2Int> notFullyScratchedGridNearby = new List<Vector2Int>();
            int gridX = currentFX.currentGrid.x;
            int gridY = currentFX.currentGrid.y;

            Vector2Int rightGrid = new Vector2Int(gridX + 1, gridY);
            Vector2Int leftGrid = new Vector2Int(gridX - 1, gridY);
            Vector2Int upGrid = new Vector2Int(gridX, gridY - 1);
            Vector2Int downGrid = new Vector2Int(gridX, gridY + 1);

            if (rightGrid.x < gridSize.x) notFullyScratchedGridNearby.Add(rightGrid);
            if (leftGrid.x >= 0) notFullyScratchedGridNearby.Add(leftGrid);
            if (upGrid.y >= 0) notFullyScratchedGridNearby.Add(upGrid);
            if (downGrid.y < gridSize.y) notFullyScratchedGridNearby.Add(downGrid);

            foreach (var grid in notFullyScratchedGridNearby.ToList())
            {
                if (scratchingStatusMatrix.GetElement(grid))
                {
                    notFullyScratchedGridNearby.Remove(grid);
                }
            }

            // randomly choose one position to generate a new vfx
            if (notFullyScratchedGridNearby.Count == 0) return;

            // int randIndex = Random.Range(0, notFullyScratchedGridNearby.Count);
            // Vector2Int spawnGrid = notFullyScratchedGridNearby[randIndex];
            Vector2Int spawnGrid = Utils.GetRandomElementFromList(notFullyScratchedGridNearby);
            Vector2 spawnPosition = gridPositionMatrix.GetElement(spawnGrid) + (Vector2)currentFX.transform.parent.position;;

            AlphaSquareFX newFx = currentFX.Replicate(currentFX.transform.position, spawnGrid);
            MoveFX(newFx, spawnPosition, spawnGrid);

            currentAlphaSquareFxList.Add(newFx);

            // StartCoroutine(ResetActionTrigger(.1f, _splitTriggered));
        }

        private void MoveFX(AlphaSquareFX fx, Vector2 destination, Vector2Int newGrid)
        {
            fx.transform.DOMove(destination, moveDuration)
                .OnStart(() =>
                {
                    isMoving = true;
                })
                .OnComplete(() =>
                {
                    isMoving = false;
                    fx.currentGrid = newGrid;
                    // TODO: trigger destroy when overlap
                    DestroyOverlappedFX();
                });
        }

        private void DestroyOverlappedFX()
        {
            print("destroy overlap FX");
            int fxCount = currentAlphaSquareFxList.Count;
            if (fxCount <= 1) return;

            for (int i = 0; i < currentAlphaSquareFxList.Count - 1; i++)
            {
                Vector2Int fxGrid = currentAlphaSquareFxList[i].currentGrid;
                // print("fxGrid" + fxGrid);
                for (int j = i + 1; j < currentAlphaSquareFxList.Count; j++)
                {
                    Vector2Int otherFxGrid = currentAlphaSquareFxList[j].currentGrid;
                    // print("otherFxGrid" + otherFxGrid);

                    if (fxGrid.Equals(otherFxGrid))
                    {
                        Destroy(currentAlphaSquareFxList[j].gameObject);
                        currentAlphaSquareFxList.Remove(currentAlphaSquareFxList[j]);
                    }
                }
            }
        }
        #endregion
    }
}