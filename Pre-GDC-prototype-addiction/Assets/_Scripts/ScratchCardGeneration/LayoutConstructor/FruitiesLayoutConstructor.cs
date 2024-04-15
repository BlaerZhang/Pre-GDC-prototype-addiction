using System;
using System.Collections.Generic;
using System.Linq;
using Interaction;
using ScratchCardGeneration.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScratchCardGeneration.LayoutConstructor
{
    public class FruitiesLayoutConstructor : MonoBehaviour, ICardLayoutConstructor
    {
        [Header("Prize Distribution")]
        public int minPrizeSplitParts;
        public int maxPrizeSplitParts;
        public int minSplitValue;

        [Header("Icon Placement")]
        // include prize number and icon sprite
        public List<Sprite> iconSprites;

        // starts in the bottom-left corner (local space)
        public Vector2 targetAreaStartPosition;
        // （row, column)
        public Vector2Int targetAreaGridSize;

        public Vector2 prizeAreaStartPosition;
        public Vector2Int prizeAreaGridSize;

        public float cellSize = 1f;
        public float targetGapLength = 0.1f;
        public float prizeGapLength = 0.1f;

        public GameObject scratchIndicator;

        public static Action<float> onScratchCardConstructed;

        // private
        private GameObject currentScratchCard;

        [HideInInspector] public VariableMatrix<int> targetIndexMatrix;
        [HideInInspector] public VariableMatrix<int> prizeIndexMatrix;

        // for alpha light effect
        [HideInInspector] public VariableMatrix<Vector2> prizeCellPositionMatrix;
        // set to true if fully scratched
        [HideInInspector] public VariableMatrix<bool> scratchingStatusMatrix;
        [HideInInspector] public List<Vector2Int> prizeWinningGridList;

        public GameObject ConstructCardLayout(float totalPrize, Vector3 generatePosition)
        {
            if (currentScratchCard != null)
            {
                Destroy(currentScratchCard);
            }
            currentScratchCard = new GameObject("newScratchCard")
            {
                transform =
                {
                    position = generatePosition
                }
            };

            scratchingStatusMatrix = new VariableMatrix<bool>(prizeAreaGridSize.x, prizeAreaGridSize.y, false);

            DistributeIcons(totalPrize);

            GenerateCardFace();

            return currentScratchCard;
        }

        GameObject ConstructIconObject(Sprite iconSprite)
        {
            GameObject iconObject = new GameObject("IconObject");
            iconObject.AddComponent<SpriteRenderer>().sprite = iconSprite;
            iconObject.transform.SetParent(currentScratchCard.transform);

            return iconObject;
        }

        private void AddFakePrizeRevealing(GameObject iconObject, Vector2Int currentGrid)
        {
            // iconObject.AddComponent<FakePrizeRevealing>();
            var indicatorObject = Instantiate(scratchIndicator, iconObject.transform.position, Quaternion.identity);
            PrizeRevealing prizeRevealing = indicatorObject.AddComponent<PrizeRevealing>();
            prizeRevealing.isWinningPrize = false;
            prizeRevealing.currentGrid = currentGrid;
            indicatorObject.transform.SetParent(iconObject.transform);
        }

        private void AddRealPrizeRevealing(GameObject iconObject, Vector2Int currentGrid, float prize)
        {
            var indicatorObject = Instantiate(scratchIndicator, iconObject.transform.position, Quaternion.identity);
            PrizeRevealing prizeRevealing = indicatorObject.AddComponent<PrizeRevealing>();
            prizeRevealing.prize = prize;
            prizeRevealing.isWinningPrize = true;
            prizeRevealing.currentGrid = currentGrid;

            indicatorObject.transform.SetParent(iconObject.transform);
        }

        /// <summary>
        /// place the icon onto the card
        /// </summary>
        private void PlaceIcons(VariableMatrix<int> iconIndexMatrix, Vector2 startPosition, float gapLength)
        {
            int row = iconIndexMatrix.GetRow();
            int col = iconIndexMatrix.GetColumn();

            Vector2 topLeftStartPosition = new Vector2(startPosition.x, startPosition.y + (row - 1) * (cellSize + gapLength));

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Vector2 cellPosition = topLeftStartPosition + new Vector2(j * (cellSize + gapLength), -i * (cellSize + gapLength));

                    int spriteIndex = iconIndexMatrix.GetElement(i, j);
                    GameObject icon = ConstructIconObject(iconSprites[spriteIndex]);

                    icon.transform.localPosition = cellPosition;
                }
            }
        }

        private void PlaceIcons(VariableMatrix<int> iconIndexMatrix, Vector2 startPosition, float gapLength, List<int> targetIndexList, List<float> priceList)
        {
            int row = iconIndexMatrix.GetRow();
            int col = iconIndexMatrix.GetColumn();
            
            int winningPrizeCounter = 0;

            Vector2 topLeftStartPosition = new Vector2(startPosition.x, startPosition.y + (row - 1) * (cellSize + gapLength));

            for (int i = 0; i < row; i++)
            {
                prizeCellPositionMatrix.AddRow();
                for (int j = 0; j < col; j++)
                {
                    Vector2 cellPosition = topLeftStartPosition + new Vector2(j * (cellSize + gapLength), -i * (cellSize + gapLength));
                    prizeCellPositionMatrix.AddElement(i, cellPosition);

                    int spriteIndex = iconIndexMatrix.GetElement(i, j);
                    GameObject icon = ConstructIconObject(iconSprites[spriteIndex]);

                    Vector2Int currentGrid = new Vector2Int(i, j);

                    if (!targetIndexList.Contains(spriteIndex))
                    {
                        AddFakePrizeRevealing(icon, currentGrid);
                    }
                    else
                    {
                        prizeWinningGridList.Add(currentGrid);
                        AddRealPrizeRevealing(icon, currentGrid, priceList[winningPrizeCounter]);
                        winningPrizeCounter++;
                    }

                    icon.transform.localPosition = cellPosition;
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

           targetIndexMatrix = Utils.ListToVariableMatrix(targetIndexList, targetY, targetX);
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

           prizeIndexMatrix = Utils.ListToVariableMatrix(prizeIndexList, prizeY, prizeX);
           prizeIndexMatrix.PrintMatrix();

           prizeCellPositionMatrix = new VariableMatrix<Vector2>();
           
           // place icons
           PlaceIcons(targetIndexMatrix, targetAreaStartPosition, targetGapLength);
           PlaceIcons(prizeIndexMatrix, prizeAreaStartPosition, prizeGapLength, targetIndexList, splitPrizes);

           prizeCellPositionMatrix.PrintMatrix();

           onScratchCardConstructed?.Invoke(totalPrize);
       }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < targetAreaGridSize.x; i++)
            {
                for (int j = 0; j < targetAreaGridSize.y; j++)
                {
                    Vector2 cellPosition = new Vector2(i * cellSize + targetGapLength * i, j * cellSize + targetGapLength * j) + targetAreaStartPosition;
                    Gizmos.DrawWireCube(cellPosition, cellSize * Vector2.one);
                }
            }

            Gizmos.color = Color.green;
            for (int i = 0; i < prizeAreaGridSize.x; i++)
            {
                for (int j = 0; j < prizeAreaGridSize.y; j++)
                {
                    Vector2 cellPosition = new Vector2(i * cellSize + prizeGapLength * i, j * cellSize + prizeGapLength * j) + prizeAreaStartPosition;
                    Gizmos.DrawWireCube(cellPosition, cellSize * Vector2.one);
                }
            }
        }

        [Header("Scratch Field Setting")]
        public GameObject scratchBackgroundPrefab;
        public GameObject scratchFieldPrefab;

        // TODO: generate scratch field according to the sprite -> set native size of the scratch card
        // TODO: dynamically generate bg position
        private void GenerateCardFace()
        {
            GameObject scratchBackground = Instantiate(scratchBackgroundPrefab, Vector3.zero, Quaternion.identity);
            scratchBackground.transform.SetParent(currentScratchCard.transform);
            scratchBackground.transform.localPosition = Vector2.zero;

            GameObject scratchFieldObject = Instantiate(scratchFieldPrefab, new Vector3(0, 0,0.01f), Quaternion.identity);
            scratchFieldObject.transform.SetParent(scratchBackground.transform);
            scratchFieldObject.transform.localPosition = Vector2.zero;
        }
    }
}