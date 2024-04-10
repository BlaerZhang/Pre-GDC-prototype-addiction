using System;
using System.Collections.Generic;
using System.Linq;
using Interaction;
using ScratchCardGeneration.Utilities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScratchCardGeneration.LayoutConstructor
{
    /// <summary>
    /// TODO: refactor the code
    /// 1. generate the sprite index list
    /// 2. use the list to generate matrix
    /// 3. use the matrix to generate the icon
    /// </summary>
    public class FruitiesLayoutConstructor : MonoBehaviour, ICardLayoutConstructor
    {

        private GameObject _currentScratchCard;

        private VariableMatrix<int> targetIndexMatrix = new VariableMatrix<int>();
        private VariableMatrix<int> prizeIndexMatrix = new VariableMatrix<int>();

        [Header("Prize Distribution")]
        public int minPrizeSplitParts;
        public int maxPrizeSplitParts;
        public int minSplitValue;

        [Header("Icon Placement")]
        // include prize number and icon sprite
        public List<Sprite> iconSprites;

        // starts at the bottom-left corner (local space)
        public Vector2 targetAreaStartPosition;
        // （row, column)
        public Vector2Int targetAreaGridSize;

        public Vector2 prizeAreaStartPosition;
        public Vector2Int prizeAreaGridSize;

        public float cellSize = 1f;
        public float targetGapLength = 0.1f;
        public float prizeGapLength = 0.1f;

        public GameObject scratchIndicator;

        public GameObject ConstructCardLayout(float totalPrize, Vector3 generatePos)
        {
            if (_currentScratchCard != null)
            {
                Destroy(_currentScratchCard);
            }
            _currentScratchCard = new GameObject("newScratchCard");

            _currentScratchCard.transform.position = generatePos;

            DistributeIcons(totalPrize);

            GenerateCardFace();

            return _currentScratchCard;
        }

        GameObject ConstructTargetIconObject(Sprite iconSprite)
        {
            GameObject iconObject = new GameObject("targetIconObject");
            iconObject.AddComponent<SpriteRenderer>().sprite = iconSprite;
            iconObject.transform.SetParent(_currentScratchCard.transform);

            return iconObject;
        }

        GameObject ConstructPrizeIconObject(Sprite iconSprite)
        {
            GameObject iconObject = new GameObject("prizeIconObject");
            iconObject.AddComponent<SpriteRenderer>().sprite = iconSprite;
            iconObject.transform.SetParent(_currentScratchCard.transform);

            iconObject.AddComponent<FakePrizeRevealing>();

            return iconObject;
        }

        GameObject ConstructPrizeIconObject(Sprite iconSprite, float prize)
        {
            GameObject iconObject = new GameObject("prizeIconObject");
            iconObject.AddComponent<SpriteRenderer>().sprite = iconSprite;
            iconObject.transform.SetParent(_currentScratchCard.transform);

            BoxCollider2D boxCollider = iconObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;

            iconObject.AddComponent<PrizeRevealing>().prize = prize;

            var indicatorObject = Instantiate(scratchIndicator, iconObject.transform.position, Quaternion.identity);
            indicatorObject.transform.SetParent(iconObject.transform);

            return iconObject;
        }

        /// <summary>
        /// place the icon onto the card
        /// </summary>
        // void PlaceIcons(List<GameObject> icons, Vector2 startPosition, int xGridAmount, int yGridAmount, float gapLength)
        // {
        //     for (int i = 0; i < xGridAmount; i++)
        //     {
        //         for (int j = 0; j < yGridAmount; j++)
        //         {
        //             Vector2 cellPosition = new Vector2(i * cellSize + gapLength * i, j * cellSize + gapLength * j) + startPosition;
        //             icons[i * yGridAmount + j].transform.localPosition = cellPosition;
        //
        //             // Instantiate(icons[i * j + j], cellPosition, Quaternion.identity);
        //         }
        //     }
        // }

        private void PlaceIcons(VariableMatrix<int> iconIndexMatrix, Vector2 startPosition, float gapLength, List<int> targetIndexList = null, bool isTarget = true)
        {
            int row = iconIndexMatrix.Row;
            int col = iconIndexMatrix.Column;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Vector2 cellPosition = new Vector2(i * cellSize + gapLength * i, j * cellSize + gapLength * j) + startPosition;
                    int spriteIndex = iconIndexMatrix.GetElement(i, j);

                    GameObject icon;

                    if (isTarget) icon = ConstructTargetIconObject(iconSprites[spriteIndex]);
                    else
                    {
                        if (targetIndexList == null) return;
                        if (!targetIndexList.Contains(spriteIndex)) icon = ConstructPrizeIconObject(iconSprites[spriteIndex]);
                        else icon = ConstructPrizeIconObject(iconSprites[spriteIndex]);
                    }

                    icon.transform.localPosition = cellPosition;
                }
            }
        }

        private VariableMatrix<T> ListToVariableMatrix<T>(List<T> list, int row, int column)
        {
            VariableMatrix<T> matrix = new VariableMatrix<T>();
            for (int i = 0; i < row; i++)
            {
                matrix.AddRow();
                for (int j = 0; j < column; j++)
                {
                    // possible error //
                    int currentIndex = i * column + j;
                    matrix.AddElement(j, list[currentIndex]);
                }
            }

            return matrix;
        }

        /// <summary>
        /// generate a icon distribution matches the total prize
        /// </summary>
        void DistributeIcons(float totalPrize)
       {
           int targetAmount = targetAreaGridSize.x * targetAreaGridSize.y;
           int spriteAmount = iconSprites.Count;

           // generate non-repeated target icons
           // var iconPoolArray = new Sprite[iconSprites.Count];
           // iconSprites.CopyTo(iconPoolArray);
           // var prizeIcons = iconPoolArray.ToList();
           //
           // List<Sprite> targetIcons = new List<Sprite>();
           // List<GameObject> targetIconObjects = new List<GameObject>();

           HashSet<int> excludedIndex = new HashSet<int>();
           List<int> targetIndexList = new List<int>();

           for (int i = 0; i < targetAmount; i++)
           {
               int randomTarget = Utils.GetRandomWithExclusions(0, spriteAmount, excludedIndex);
               targetIndexList.Add(randomTarget);
               // targetIndexMatrix.AddElement(i, randomTarget);
               excludedIndex.Add(randomTarget);
           }

           int targetX = targetAreaGridSize.x;
           int targetY = targetAreaGridSize.y;

           targetIndexMatrix = ListToVariableMatrix(targetIndexList, targetY, targetX);

           print("targetIndexMatrix");

           print($"targetIndexMatrix.Column: {targetIndexMatrix.Column}");
           print($"targetIndexMatrix.row: {targetIndexMatrix.Row}");
           targetIndexMatrix.PrintMatrix();

           for (int i = 0; i < targetY; i++)
           {
               targetIndexMatrix.AddRow();
               for (int j = 0; j < targetX; j++)
               {
                   int randomTarget = Utils.GetRandomWithExclusions(0, spriteAmount, excludedIndex);
                   targetIndexMatrix.AddElement(i, randomTarget);
                   excludedIndex.Add(randomTarget);
               }
           }

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

           prizeIndexMatrix = ListToVariableMatrix(prizeIndexList, prizeY, prizeX);

           print("prizeIndexMatrix");
           prizeIndexMatrix.PrintMatrix();


           // place icons
           // PlaceIcons(targetIndexMatrix, targetAreaStartPosition, targetGapLength);
           // PlaceIcons(prizeIndexMatrix, prizeAreaStartPosition, prizeGapLength, targetIndexList, false);

           // // randomly select target icons
           // for (int i = 0; i < targetAmount; i++)
           // {
           //     var currentIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];
           //     targetIcons.Add(currentIcon);
           //     targetIconObjects.Add(ConstructTargetIconObject(currentIcon));
           //     prizeIcons.Remove(currentIcon);
           // }
           //
           // // construct the icon matrix along with the prizes
           // List<GameObject> prizeMatrix = new List<GameObject>();
           // int prizeAmount = prizeAreaGridSize.x * prizeAreaGridSize.y;
           //
           // // check whether the current card wins prize
           // if (totalPrize == 0)
           // {
           //     for (int i = 0; i < prizeAmount; i++)
           //     {
           //         Sprite randIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];
           //
           //         GameObject iconObject = ConstructPrizeIconObject(randIcon);
           //         prizeMatrix.Add(iconObject);
           //     }
           // }
           // else
           // {
           //     // split total prizes into multiple small prizes
           //     var splitPrizes = Utils.SplitNumbers(totalPrize, minPrizeSplitParts, maxPrizeSplitParts, minSplitValue);
           //     int winningPrizesAmount = splitPrizes.Count;
           //     Debug.Log($"winningPrizesAmount: {winningPrizesAmount}");
           //
           //     // fill the matrix with random prize numbers
           //     for (int i = 0; i < prizeAmount - winningPrizesAmount; i++)
           //     {
           //         Sprite randIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];
           //
           //         GameObject iconObject = ConstructPrizeIconObject(randIcon);
           //         prizeMatrix.Add(iconObject);
           //     }
           //
           //     // insert winning icons and prizes into the matrix
           //     for (int i = 0; i < winningPrizesAmount; i++)
           //     {
           //         Sprite randIcon = targetIcons[Random.Range(0, targetIcons.Count)];
           //
           //         int randIndex = Random.Range(0, prizeMatrix.Count);
           //         GameObject iconObject = ConstructPrizeIconObject(randIcon, splitPrizes[i]);
           //         prizeMatrix.Insert(randIndex, iconObject);
           //     }
           // }
           //
           // // place icons according to the matrix
           // PlaceIcons(targetIconObjects, targetAreaStartPosition, targetAreaGridSize.x, targetAreaGridSize.y, targetGapLength);
           // PlaceIcons(prizeMatrix, prizeAreaStartPosition, prizeAreaGridSize.x, prizeAreaGridSize.y, prizeGapLength);
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
        public void GenerateCardFace()
        {
            GameObject scratchBackground = Instantiate(scratchBackgroundPrefab, Vector3.zero, Quaternion.identity);
            scratchBackground.transform.SetParent(_currentScratchCard.transform);
            scratchBackground.transform.localPosition = Vector2.zero;

            GameObject scratchFieldObject = Instantiate(scratchFieldPrefab, new Vector3(0, 0,0.01f), Quaternion.identity);
            scratchFieldObject.transform.SetParent(scratchBackground.transform);
            scratchFieldObject.transform.localPosition = Vector2.zero;
        }

    }
}