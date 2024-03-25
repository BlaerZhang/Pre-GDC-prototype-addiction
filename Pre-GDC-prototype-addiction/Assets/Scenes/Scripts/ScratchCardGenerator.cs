using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

/// <summary>
/// generates a certain kind of scratch card visually
/// </summary>
public class ScratchCardGenerator : MonoBehaviour
{
    [Serializable]
    public class PrizeDistribution
    {
        public float prize;
        public float probability;
    }

    public class CustomComparer : IComparer<PrizeDistribution>
    {
        public int Compare(PrizeDistribution x, PrizeDistribution y)
        {
            return x.probability.CompareTo(y.probability);
        }
    }

    [Header("Scratch Card Setting")]
    // public GameObject scratchCardPrefab;
    public float price;
    // public float responseRate;
    public List<PrizeDistribution> prizeDistributions;

    public float costThreshold;
    public float winningProbabilityOverThreshold;

    // global among the same kind
    private float totalCostBeforeWinning = 0;
    private float currentWinningProbability;

    [Header("Icon Placement")]
    // include prize number and icon sprite
    public List<Sprite> iconSprites;

    public Vector2 iconPrizePosOffset = new Vector2(0, -1);

    // starts at the bottom-left corner (local space)
    public Vector2 targetAreaStartPosition;
    // （row, column)
    public Vector2Int targetAreaGridSize;

    public Vector2 prizeAreaStartPosition;
    public Vector2Int prizeAreaGridSize;

    public float cellSize = 1f;
    public float gapLength = 0.1f;

    void Start()
    {
        GenerateScratchCard();
    }

    void GenerateScratchCard()
    {
        AdjustWinningProbability();
        DistributeIcons();
    }

    /// <summary>
    /// adjust winning probability according to the previous spent
    /// </summary>
    void AdjustWinningProbability()
    {
        if (totalCostBeforeWinning > costThreshold)
        {
            currentWinningProbability = winningProbabilityOverThreshold;
            totalCostBeforeWinning = 0;
        }
    }


    /// <summary>
    /// generate the total prize of the current card
    /// </summary>
    float GenerateCurrentCardPrize()
    {
        // win
        if (Random.Range(0, 1) <= currentWinningProbability)
        {
            // generate prize
            prizeDistributions.Sort(new CustomComparer());

            float rand = Random.value;
            foreach (var d in prizeDistributions)
            {
                if (rand <= d.probability)
                    return d.prize;
            }
        }
        return 0;
    }

    //-------------------------------------------------
    GameObject ConstructIconObject(Sprite iconSprite)
    {
        GameObject iconObject = new GameObject("targetIconObject");
        iconObject.AddComponent<SpriteRenderer>().sprite = iconSprite;
        iconObject.transform.parent = transform;

        return iconObject;
    }

    GameObject ConstructIconObject(Vector2 iconPrizePosOffset, Sprite iconSprite, float prize)
    {
        GameObject iconObject = new GameObject("prizeIconObject");
        iconObject.AddComponent<SpriteRenderer>().sprite = iconSprite;
        iconObject.transform.parent = transform;

        TMP_Text textObject = new TextMeshPro();
        textObject.text = prize.ToString();
        textObject.transform.parent = iconObject.transform;
        textObject.transform.position += (Vector3)iconPrizePosOffset;

        return iconObject;
    }

    /// <summary>
    /// possibly split the total prize, and place icon and prize together
    /// </summary>
    void DistributeIcons()
    {
        int targetAmount = targetAreaGridSize.x * targetAreaGridSize.y;

        // generate non-repeated target icons
        var iconPoolArray = new Sprite[iconSprites.Count];
        iconSprites.CopyTo(iconPoolArray);
        var iconPool = iconPoolArray.ToList();

        List<Sprite> targetIcons = new List<Sprite>();
        List<GameObject> targetIconObjects = new List<GameObject>();

        // randomly select target icons
        for (int i = 0; i < targetAmount; i++)
        {
            var currentIcon = iconPool[Random.Range(0, iconPool.Count)];
            targetIcons.Add(currentIcon);
            targetIconObjects.Add(ConstructIconObject(currentIcon));
            iconPool.Remove(currentIcon);
        }

        // split total prizes into multiple small prizes
        var splitPrizes = Utils.SplitNumbers(GenerateCurrentCardPrize(), 3, 5, 3);

        // construct the icon matrix along with the prizes
        int prizeAmount = prizeAreaGridSize.x * prizeAreaGridSize.y;
        int winningPrizesAmount = splitPrizes.Count;

        List<GameObject> prizeMatrix = new List<GameObject>();

        // fill the matrix with random prize numbers and
        for (int i = 0; i < prizeAmount - winningPrizesAmount; i++)
        {
            Sprite randIcon = iconPool[Random.Range(0, iconPool.Count - 1)];
            float randPrize = Random.Range(0, 10000);

            GameObject iconObject = ConstructIconObject(iconPrizePosOffset, randIcon, randPrize);
            prizeMatrix.Add(iconObject);
        }

        // insert winning icons and prizes into the matrix
        for (int i = 0; i < winningPrizesAmount; i++)
        {
            Sprite randIcon = targetIcons[Random.Range(0, targetIcons.Count - 1)];
            int randIndex = Random.Range(0, prizeMatrix.Count - 1);

            GameObject iconObject = ConstructIconObject(iconPrizePosOffset, randIcon, splitPrizes[i]);
            prizeMatrix.Insert(randIndex, iconObject);
        }

        // place icons according to the matrix
        PlaceIcons(targetIconObjects, targetAreaStartPosition, targetAreaGridSize.x, targetAreaGridSize.y);
        PlaceIcons(prizeMatrix, prizeAreaStartPosition, prizeAreaGridSize.x, prizeAreaGridSize.y);
    }

    /// <summary>
    /// place the icon onto the card
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="xGridAmount"></param>
    /// <param name="yGridAmount"></param>
    void PlaceIcons(List<GameObject> icons, Vector2 startPosition, int xGridAmount, int yGridAmount)
    {
        for (int i = 0; i < xGridAmount; i++)
        {
            for (int j = 0; j < yGridAmount; j++)
            {
                Vector2 cellPosition = new Vector2(i * cellSize + gapLength * i, j * cellSize + gapLength * j) + startPosition;
                Instantiate(icons[i * j + j], cellPosition, Quaternion.identity);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < targetAreaGridSize.x; i++)
        {
            for (int j = 0; j < targetAreaGridSize.y; j++)
            {
                Vector2 cellPosition = new Vector2(i * cellSize + gapLength * i, j * cellSize + gapLength * j) + targetAreaStartPosition;
                Gizmos.DrawWireCube(cellPosition, cellSize * Vector2.one);
            }
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < prizeAreaGridSize.x; i++)
        {
            for (int j = 0; j < prizeAreaGridSize.y; j++)
            {
                Vector2 cellPosition = new Vector2(i * cellSize + gapLength * i, j * cellSize + gapLength * j) + prizeAreaStartPosition;
                Gizmos.DrawWireCube(cellPosition, cellSize * Vector2.one);
            }
        }
    }
}
