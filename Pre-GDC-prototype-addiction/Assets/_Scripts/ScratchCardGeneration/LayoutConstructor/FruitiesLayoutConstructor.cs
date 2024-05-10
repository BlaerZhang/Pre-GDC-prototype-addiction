using System;
using System.Collections.Generic;
using System.Linq;
using Interaction;
using ScratchCardGeneration.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ScratchCardGeneration.LayoutConstructor
{
    public class FruitiesLayoutConstructor : MonoBehaviour, ICardLayoutConstructor
    {
        [Title("Prize Distribution")]
        public int minPrizeSplitParts;
        public int maxPrizeSplitParts;
        private int minSplitValue;

        [Title("Icon Placement")]
        // include prize number and icon sprite
        public List<Sprite> iconSprites;

        [InfoBox("Enter the scale of the scratch card prefab that will be generated later")]
        public float scratchCardScale = 1;

        // starts in the bottom-left corner (local space)
        public Vector2 targetAreaStartPosition;
        // （row, column)
        public Vector2Int targetAreaGridSize;

        public Vector2 prizeAreaStartPosition;
        public Vector2Int prizeAreaGridSize;

        public float cellSize = 1f;
        public float targetGapLength = 0.1f;
        public float prizeGapLength = 0.1f;

        public GameObject scratchPrizeGrid;
        public GameObject scratchTargetGrid;

        public static Action<float, float> onScratchCardConstructed;

        // private
        private GameObject currentScratchCard;
        private VariableMatrix<int> targetIndexMatrix;
        private VariableMatrix<int> prizeIndexMatrix;

        // for alpha light effect
        [HideInInspector] public VariableMatrix<Vector2> PrizeCellPositionMatrix;
        [HideInInspector] public VariableMatrix<bool> ScratchingStatusMatrix;
        [HideInInspector] public VariableMatrix<GameObject> IconObjectMatrix;
        [HideInInspector] public List<Vector2Int> prizeWinningGridList;
        [HideInInspector] public List<Vector2Int> notWinningGridList;

        public GameObject ConstructCardLayout(float totalPrize, float price, Vector3 generatePosition)
        {
            if (currentScratchCard != null)
            {
                Destroy(currentScratchCard);
            }
            currentScratchCard = new GameObject("currentScratchCard")
            {
                transform =
                {
                    position = generatePosition
                }
            };

            InitData();

            SetPrizeMinSplitValue(totalPrize);

            DistributeIcons(totalPrize);

            onScratchCardConstructed?.Invoke(totalPrize, price);

            return currentScratchCard;
        }

        private void InitData()
        {
            prizeWinningGridList = new List<Vector2Int>();
            notWinningGridList = new List<Vector2Int>();
            PrizeCellPositionMatrix = new VariableMatrix<Vector2>();
            ScratchingStatusMatrix = new VariableMatrix<bool>(prizeAreaGridSize.x, prizeAreaGridSize.y, false);
            IconObjectMatrix = new VariableMatrix<GameObject>(prizeAreaGridSize.x, prizeAreaGridSize.y);
        }

        private void SetPrizeMinSplitValue(float prize)
        {

            minSplitValue = (int)Mathf.Pow(10, Utils.GetNumberPlace((int)prize));
            print("minSplitValue: " + minSplitValue);
        }

        GameObject ConstructIconObject(Sprite iconSprite, bool isTargetIcon)
        {
            GameObject iconObject = new GameObject("IconObject");
            var spriteRenderer = iconObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = iconSprite;

            var sortingGroup = iconObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Scratch Card";
            sortingGroup.sortingOrder = 2;

            iconObject.transform.SetParent(currentScratchCard.transform);

            var indicatorObject = isTargetIcon
                ? Instantiate(scratchTargetGrid, iconObject.transform.position, Quaternion.identity)
                : Instantiate(scratchPrizeGrid, iconObject.transform.position, Quaternion.identity);
            indicatorObject.transform.SetParent(iconObject.transform);

            return indicatorObject;
        }

        private void AddFakePrizeRevealing(GameObject indicatorObject, Vector2Int currentGrid)
        {
            if (!indicatorObject.TryGetComponent(out PrizeRevealing prizeRevealing)) indicatorObject.AddComponent<PrizeRevealing>();
            prizeRevealing.isWinningPrize = false;
            prizeRevealing.currentGrid = currentGrid;
        }

        private void AddRealPrizeRevealing(GameObject indicatorObject, Vector2Int currentGrid, float prize)
        {
            if (!indicatorObject.TryGetComponent(out PrizeRevealing prizeRevealing)) indicatorObject.AddComponent<PrizeRevealing>();
            prizeRevealing.prize = prize;
            prizeRevealing.isWinningPrize = true;
            prizeRevealing.currentGrid = currentGrid;
        }

        /// <summary>
        /// place the icon onto the target area
        /// </summary>
        private void PlaceIcons(VariableMatrix<int> iconIndexMatrix)
        {
            int row = targetAreaGridSize.x;
            int col = targetAreaGridSize.y;

            Vector2 topLeftStartPosition = new Vector2(targetAreaStartPosition.x, targetAreaStartPosition.y + (row - 1) * (cellSize + targetGapLength));

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Vector2 cellPosition = topLeftStartPosition + new Vector2(j * (cellSize + targetGapLength), -i * (cellSize + targetGapLength));

                    int spriteIndex = iconIndexMatrix.GetElement(i, j);
                    GameObject indicatorObject = ConstructIconObject(iconSprites[spriteIndex], true);

                    GameObject iconObject = indicatorObject.transform.parent.gameObject;
                    iconObject.transform.localPosition = cellPosition;
                }
            }
        }

        /// <summary>
        /// place the icon onto the prize area
        /// </summary>
        private void PlaceIcons(VariableMatrix<int> iconIndexMatrix, List<int> targetIndexList, List<float> prizeList)
        {
            int row = prizeAreaGridSize.x;
            int col = prizeAreaGridSize.y;
            
            int winningPrizeCounter = 0;

            Vector2 topLeftStartPosition = new Vector2(prizeAreaStartPosition.x, prizeAreaStartPosition.y + (row - 1) * (cellSize + prizeGapLength));

            for (int i = 0; i < row; i++)
            {
                PrizeCellPositionMatrix.AddRow();
                for (int j = 0; j < col; j++)
                {
                    Vector2 cellPosition = topLeftStartPosition + new Vector2(j * (cellSize + prizeGapLength), -i * (cellSize + prizeGapLength));
                    PrizeCellPositionMatrix.AddElement(i, cellPosition * scratchCardScale);

                    int spriteIndex = iconIndexMatrix.GetElement(i, j);
                    GameObject indicatorObject = ConstructIconObject(iconSprites[spriteIndex], false);

                    GameObject iconObject = indicatorObject.transform.parent.gameObject;
                    IconObjectMatrix.SetElement(i, j, iconObject);

                    Vector2Int currentGrid = new Vector2Int(i, j);

                    if (!targetIndexList.Contains(spriteIndex))
                    {
                        AddFakePrizeRevealing(indicatorObject, currentGrid);
                        notWinningGridList.Add(currentGrid);
                    }
                    else
                    {
                        prizeWinningGridList.Add(currentGrid);
                        AddRealPrizeRevealing(indicatorObject, currentGrid, prizeList[winningPrizeCounter]);
                        winningPrizeCounter++;
                    }

                    indicatorObject.transform.parent.localPosition = cellPosition;
                }
            }
        }

        /// <summary>
        /// generate a icon distribution matches the total prize
        /// </summary>
        void DistributeIcons(float totalPrize)
       {
           int targetAmount = targetAreaGridSize.x * targetAreaGridSize.y;
           int spriteAmount = iconSprites.Count;

           HashSet<int> excludedIndex = new HashSet<int>();
           List<int> targetIndexList = new List<int>();

           for (int i = 0; i < targetAmount; i++)
           {
               int randomTarget = Utils.GetRandomWithExclusions(0, spriteAmount, excludedIndex);
               targetIndexList.Add(randomTarget);
               excludedIndex.Add(randomTarget);
           }

           int targetX = targetAreaGridSize.x;
           int targetY = targetAreaGridSize.y;

           targetIndexMatrix = Utils.ListToVariableMatrix(targetIndexList, targetX, targetY);
           targetIndexMatrix.PrintMatrix();

           int prizeAmount = prizeAreaGridSize.x * prizeAreaGridSize.y;

           List<int> prizeIndexList = new List<int>();

           var splitPrizes = Utils.SplitNumbers(totalPrize, minPrizeSplitParts, maxPrizeSplitParts, minSplitValue);
           int winningPrizesAmount = splitPrizes.Count;

           // fill the matrix with random prize numbers
           for (int i = 0; i < prizeAmount - winningPrizesAmount; i++)
           {
               int randomPrize = Utils.GetRandomWithExclusions(0, spriteAmount, excludedIndex);
               prizeIndexList.Add(randomPrize);
           }
           
           // insert winning icons and prizes into the matrix
           for (int i = 0; i < winningPrizesAmount; i++)
           {
               int randomWinningPrize = excludedIndex.ToList()[Random.Range(0, excludedIndex.Count)];

               int randIndexInList = Random.Range(0, prizeIndexList.Count);
               
               prizeIndexList.Insert(randIndexInList, randomWinningPrize);
           }

           int prizeX = prizeAreaGridSize.x;
           int prizeY = prizeAreaGridSize.y;

           prizeIndexMatrix = Utils.ListToVariableMatrix(prizeIndexList, prizeX, prizeY);
           prizeIndexMatrix.PrintMatrix();
           
           // place icons
           PlaceIcons(targetIndexMatrix);
           PlaceIcons(prizeIndexMatrix, targetIndexList, splitPrizes);

           PrizeCellPositionMatrix.PrintMatrix();
       }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < targetAreaGridSize.x; i++)
            {
                for (int j = 0; j < targetAreaGridSize.y; j++)
                {
                    Vector2 cellPosition = new Vector2(j * (cellSize + targetGapLength), i * (cellSize + targetGapLength)) + targetAreaStartPosition;
                    Gizmos.DrawWireCube(cellPosition, cellSize * Vector2.one);
                }
            }

            Gizmos.color = Color.green;
            for (int i = 0; i < prizeAreaGridSize.x; i++)
            {
                for (int j = 0; j < prizeAreaGridSize.y; j++)
                {
                    Vector2 cellPosition = new Vector2(j * (cellSize + prizeGapLength), i * (cellSize + prizeGapLength)) + prizeAreaStartPosition;
                    Gizmos.DrawWireCube(cellPosition, cellSize * Vector2.one);
                }
            }
        }
    }
}