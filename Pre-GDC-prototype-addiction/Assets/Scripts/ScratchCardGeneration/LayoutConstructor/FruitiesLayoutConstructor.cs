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
    public class FruitiesLayoutConstructor : MonoBehaviour, ICardLayoutConstructor
    {

        private GameObject _currentScratchCard;

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

            var indicatorObject = Instantiate(scratchIndicator, iconObject.transform.localPosition, Quaternion.identity);
            indicatorObject.transform.SetParent(iconObject.transform);

            return iconObject;
        }

        /// <summary>
        /// place the icon onto the card
        /// </summary>
        void PlaceIcons(List<GameObject> icons, Vector2 startPosition, int xGridAmount, int yGridAmount, float gapLength)
        {
            for (int i = 0; i < xGridAmount; i++)
            {
                for (int j = 0; j < yGridAmount; j++)
                {
                    Vector2 cellPosition = new Vector2(i * cellSize + gapLength * i, j * cellSize + gapLength * j) + startPosition;
                    icons[i * yGridAmount + j].transform.localPosition = cellPosition;
                    // Instantiate(icons[i * j + j], cellPosition, Quaternion.identity);
                }
            }
        }

        /// <summary>
        /// generate a icon distribution matches the total prize
        /// </summary>
        void DistributeIcons(float totalPrize)
       {
           int targetAmount = targetAreaGridSize.x * targetAreaGridSize.y;

           // generate non-repeated target icons
           var iconPoolArray = new Sprite[iconSprites.Count];
           iconSprites.CopyTo(iconPoolArray);
           var prizeIcons = iconPoolArray.ToList();

           List<Sprite> targetIcons = new List<Sprite>();
           List<GameObject> targetIconObjects = new List<GameObject>();

           // randomly select target icons
           for (int i = 0; i < targetAmount; i++)
           {
               var currentIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];
               targetIcons.Add(currentIcon);
               targetIconObjects.Add(ConstructTargetIconObject(currentIcon));
               prizeIcons.Remove(currentIcon);
           }

           // construct the icon matrix along with the prizes
           List<GameObject> prizeMatrix = new List<GameObject>();
           int prizeAmount = prizeAreaGridSize.x * prizeAreaGridSize.y;

           // check whether the current card wins prize
           if (totalPrize == 0)
           {
               for (int i = 0; i < prizeAmount; i++)
               {
                   Sprite randIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];

                   GameObject iconObject = ConstructPrizeIconObject(randIcon);
                   prizeMatrix.Add(iconObject);
               }
           }
           else
           {
               // split total prizes into multiple small prizes
               var splitPrizes = Utils.SplitNumbers(totalPrize, minPrizeSplitParts, maxPrizeSplitParts, minSplitValue);
               int winningPrizesAmount = splitPrizes.Count;
               Debug.Log($"winningPrizesAmount: {winningPrizesAmount}");

               // fill the matrix with random prize numbers
               for (int i = 0; i < prizeAmount - winningPrizesAmount; i++)
               {
                   Sprite randIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];

                   GameObject iconObject = ConstructPrizeIconObject(randIcon);
                   prizeMatrix.Add(iconObject);
               }

               // insert winning icons and prizes into the matrix
               for (int i = 0; i < winningPrizesAmount; i++)
               {
                   Sprite randIcon = targetIcons[Random.Range(0, targetIcons.Count)];

                   int randIndex = Random.Range(0, prizeMatrix.Count);
                   GameObject iconObject = ConstructPrizeIconObject(randIcon, splitPrizes[i]);
                   prizeMatrix.Insert(randIndex, iconObject);
               }
           }

           // place icons according to the matrix
           PlaceIcons(targetIconObjects, targetAreaStartPosition, targetAreaGridSize.x, targetAreaGridSize.y, targetGapLength);
           PlaceIcons(prizeMatrix, prizeAreaStartPosition, prizeAreaGridSize.x, prizeAreaGridSize.y, prizeGapLength);
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