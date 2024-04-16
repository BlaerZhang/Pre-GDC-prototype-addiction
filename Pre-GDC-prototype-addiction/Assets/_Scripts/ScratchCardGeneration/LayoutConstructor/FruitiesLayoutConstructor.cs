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

        public static Action<float, float> onScratchCardConstructed;

        // private
        private GameObject currentScratchCard;

        [HideInInspector] public VariableMatrix<int> TargetIndexMatrix;
        [HideInInspector] public VariableMatrix<int> PrizeIndexMatrix;

        // for alpha light effect
        [HideInInspector] public VariableMatrix<Vector2> PrizeCellPositionMatrix;
        // set to true if fully scratched
        [HideInInspector] public VariableMatrix<bool> ScratchingStatusMatrix;
        [HideInInspector] public List<Vector2Int> prizeWinningGridList;

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

            ScratchingStatusMatrix = new VariableMatrix<bool>(prizeAreaGridSize.x, prizeAreaGridSize.y, false);

            DistributeIcons(totalPrize);

            onScratchCardConstructed?.Invoke(totalPrize, price);

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
                    GameObject icon = ConstructIconObject(iconSprites[spriteIndex]);

                    icon.transform.localPosition = cellPosition;
                }
            }
        }

        private void PlaceIcons(VariableMatrix<int> iconIndexMatrix, List<int> targetIndexList, List<float> priceList)
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
                    PrizeCellPositionMatrix.AddElement(i, cellPosition);

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

           TargetIndexMatrix = Utils.ListToVariableMatrix(targetIndexList, targetX, targetY);
           TargetIndexMatrix.PrintMatrix();

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

           PrizeIndexMatrix = Utils.ListToVariableMatrix(prizeIndexList, prizeX, prizeY);
           PrizeIndexMatrix.PrintMatrix();

           PrizeCellPositionMatrix = new VariableMatrix<Vector2>();
           
           // place icons
           PlaceIcons(TargetIndexMatrix);
           PlaceIcons(PrizeIndexMatrix, targetIndexList, splitPrizes);

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