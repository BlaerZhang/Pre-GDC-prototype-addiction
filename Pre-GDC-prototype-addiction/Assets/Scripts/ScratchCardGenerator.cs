using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interaction;
using ScratchCardAsset;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using Unity.Mathematics;

/// <summary>
/// generates a certain kind of scratch card visually
/// </summary>
public class ScratchCardGenerator : SerializedMonoBehaviour
{
    [Header("Scratch Card Setting")]
    public float price;
    // public float responseRate;
    // the possible prize when player wins
    [DictionaryDrawerSettings(KeyLabel = "Prize", ValueLabel = "Probability")]
    public Dictionary<int, float> basePrizeDistributions;
    public Dictionary<int, float> actualPrizeDistributions;

    public float costThreshold;
    public float winningProbabilityOverThreshold = 0.2f;

    // global among the same kind
    private GameObject currentScratchCard;
    // private float totalCostBeforeWinning = 0;
    // private float currentWinningProbability;

    [Header("Prize Distribution")]
    public int minPrizeSplitParts;
    public int maxPrizeSplitParts;
    public int minSplitValue;

    public List<int> possibleNumberPlaces;

    [HideInInspector] public float currentCardPrize;

    void Start()
    {
        // actualPrizeDistributions = Utils.DeepCopyDictionary(basePrizeDistributions);
        GenerateScratchCard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateScratchCard();
        }
    }

    void GenerateScratchCard()
    {
        actualPrizeDistributions = Utils.DeepCopyDictionary(basePrizeDistributions);

        // TODO: purchase card, spend gold
        GameManager.Instance.totalCostBeforeWinning += price;

        if (currentScratchCard != null)
        {
            Destroy(currentScratchCard);
        }
        currentScratchCard = new GameObject("newScratchCard");
        currentScratchCard.transform.SetParent(transform);

        print($"totalCostBeforeWinning: {GameManager.Instance.totalCostBeforeWinning}");

        AdjustWinningProbability();
        DistributeIcons();

        GenerateScratchField();
    }

    /// <summary>
    /// adjust winning probability according to the previous spent
    /// </summary>
    void AdjustWinningProbability()
    {
        if (GameManager.Instance.totalCostBeforeWinning >= costThreshold)
        {
            int randIndex = Random.Range(1, basePrizeDistributions.Count);
            actualPrizeDistributions = Utils.AdjustProbabilityRatio(basePrizeDistributions, basePrizeDistributions.Keys.ElementAt(randIndex), winningProbabilityOverThreshold);
            // actualPrizeDistributions = Utils.AdjustProbabilityRatio(basePrizeDistributions, 1000, winningProbabilityOverThreshold);
        }
    }


    /// <summary>
    /// generate the total prize of the current card
    /// </summary>
    float GenerateCurrentCardPrize()
    {
        // order the probability by ascending
        var sortedDistribution = actualPrizeDistributions.OrderBy(pair => pair.Value);

        float rand = Random.value;
        foreach (var d in sortedDistribution)
        {
            if (rand <= d.Value)
            {
                // return to the normal distribution after winning
                if (d.Key != 0)
                {
                    print($"winning prize: {d.Key}");
                    // actualPrizeDistributions = Utils.DeepCopyDictionary(basePrizeDistributions);
                    GameManager.Instance.totalCostBeforeWinning = 0;
                }
                return d.Key;
            }
        }
        return 0;
    }

    //-------------------------------------------------
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
    public float targetGapLength = 0.1f;
    public float prizeGapLength = 0.1f;

    public GameObject winningParticle;

    public GameObject scratchIndicator;

    GameObject ConstructIconObject(Sprite iconSprite)
    {
        GameObject iconObject = new GameObject("targetIconObject");
        iconObject.AddComponent<SpriteRenderer>().sprite = iconSprite;
        iconObject.transform.SetParent(currentScratchCard.transform);

        return iconObject;
    }

    GameObject ConstructIconObject(Vector2 iconPrizePosOffset, Sprite iconSprite, float prize, bool isWinning = false)
    {
        GameObject iconObject = new GameObject("prizeIconObject");
        iconObject.AddComponent<SpriteRenderer>().sprite = iconSprite;
        iconObject.transform.SetParent(currentScratchCard.transform);

        // GameObject textObject = new GameObject("prize");
        // TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
        // textObject.GetComponent<RectTransform>().sizeDelta = Vector2.one;
        // textMeshPro.text = prize.ToString();
        // textMeshPro.color = Color.red;
        // textMeshPro.fontSize = 2;
        // textMeshPro.alignment = TextAlignmentOptions.Top;
        // textObject.transform.SetParent(iconObject.transform);
        // textObject.transform.position += (Vector3)iconPrizePosOffset;

        if (isWinning)
        {
            BoxCollider2D boxCollider = iconObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;

            iconObject.AddComponent<PrizeRevealing>().prize = prize;

            var indicatorObject = Instantiate(scratchIndicator, iconObject.transform.position, Quaternion.identity);
            indicatorObject.transform.SetParent(iconObject.transform);

            // var particle = Instantiate(winningParticle, iconObject.transform.position, Quaternion.identity);
            // particle.transform.SetParent(iconObject.transform);
            // particle.transform.localPosition = new Vector3(0, 0, -0.5f);
            // particle.GetComponent<ParticleSystem>().Play();
            // print("shinning icon!");
        }

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
        var prizeIcons = iconPoolArray.ToList();

        List<Sprite> targetIcons = new List<Sprite>();
        List<GameObject> targetIconObjects = new List<GameObject>();

        // randomly select target icons
        for (int i = 0; i < targetAmount; i++)
        {
            var currentIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];
            targetIcons.Add(currentIcon);
            targetIconObjects.Add(ConstructIconObject(currentIcon));
            prizeIcons.Remove(currentIcon);
        }

        currentCardPrize  = GenerateCurrentCardPrize();

        // construct the icon matrix along with the prizes
        List<GameObject> prizeMatrix = new List<GameObject>();
        int prizeAmount = prizeAreaGridSize.x * prizeAreaGridSize.y;

        // check whether the current card wins prize
        if (currentCardPrize == 0)
        {
            for (int i = 0; i < prizeAmount; i++)
            {
                Sprite randIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];
                float randPrize = Utils.GenerateCleanNumber(possibleNumberPlaces);

                GameObject iconObject = ConstructIconObject(iconPrizePosOffset, randIcon, randPrize);
                prizeMatrix.Add(iconObject);
            }
        }
        else
        {
            // split total prizes into multiple small prizes
            var splitPrizes = Utils.SplitNumbers(currentCardPrize, minPrizeSplitParts, maxPrizeSplitParts, minSplitValue);
            int winningPrizesAmount = splitPrizes.Count;
            print($"winningPrizesAmount: {winningPrizesAmount}");

            // fill the matrix with random prize numbers
            for (int i = 0; i < prizeAmount - winningPrizesAmount; i++)
            {
                Sprite randIcon = prizeIcons[Random.Range(0, prizeIcons.Count)];
                float randPrize = Utils.GenerateCleanNumber(possibleNumberPlaces);

                GameObject iconObject = ConstructIconObject(iconPrizePosOffset, randIcon, randPrize);
                prizeMatrix.Add(iconObject);
            }

            // insert winning icons and prizes into the matrix
            for (int i = 0; i < winningPrizesAmount; i++)
            {
                Sprite randIcon = targetIcons[Random.Range(0, targetIcons.Count)];

                int randIndex = Random.Range(0, prizeMatrix.Count);
                GameObject iconObject = ConstructIconObject(iconPrizePosOffset, randIcon, splitPrizes[i], true);
                prizeMatrix.Insert(randIndex, iconObject);
            }
        }

        // place icons according to the matrix
        PlaceIcons(targetIconObjects, targetAreaStartPosition, targetAreaGridSize.x, targetAreaGridSize.y, targetGapLength);
        PlaceIcons(prizeMatrix, prizeAreaStartPosition, prizeAreaGridSize.x, prizeAreaGridSize.y, prizeGapLength);
    }

    /// <summary>
    /// place the icon onto the card
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="xGridAmount"></param>
    /// <param name="yGridAmount"></param>
    void PlaceIcons(List<GameObject> icons, Vector2 startPosition, int xGridAmount, int yGridAmount, float gapLength)
    {
        for (int i = 0; i < xGridAmount; i++)
        {
            for (int j = 0; j < yGridAmount; j++)
            {
                Vector2 cellPosition = new Vector2(i * cellSize + gapLength * i, j * cellSize + gapLength * j) + startPosition;
                icons[i * yGridAmount + j].transform.position = cellPosition;
                // Instantiate(icons[i * j + j], cellPosition, Quaternion.identity);
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
    void GenerateScratchField()
    {
        GameObject scratchBackground = Instantiate(scratchBackgroundPrefab, Vector3.zero, quaternion.identity);
        scratchBackground.transform.SetParent(currentScratchCard.transform);

        GameObject scratchFieldObject = Instantiate(scratchFieldPrefab, Vector3.zero, quaternion.identity);
        scratchFieldObject.transform.SetParent(currentScratchCard.transform);

        // Vector3 targetFieldPos = new Vector3(0, targetAreaStartPosition.y + cellSize * Mathf.FloorToInt(targetAreaGridSize.y / 2), -1);
        // GameObject targetScratchField = Instantiate(scratchFieldPrefab, targetFieldPos, Quaternion.identity);
        // // scratchFieldPrefab.GetComponent<ScratchCardManager>().SetNativeSize();
        // // change the size
        // SpriteRenderer targetAreaSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // ResizeSpriteToTargetSize(targetScratchField.transform, targetAreaSpriteRenderer, targetAreaGridSize);
        // // targetScratchField.transform.localScale = new Vector3(targetAreaGridSize.x, targetAreaGridSize.y, 1);
        // targetScratchField.transform.SetParent(currentScratchCard.transform);
        //
        //
        // Vector3 prizeFieldPos = new Vector3(0, prizeAreaStartPosition.y + cellSize * Mathf.FloorToInt(prizeAreaGridSize.y / 2), -1);
        // GameObject prizeScratchField = Instantiate(scratchFieldPrefab, prizeFieldPos, Quaternion.identity);
        // // prizeScratchField.GetComponent<ScratchCardManager>().SetNativeSize();
        // // change the size
        // SpriteRenderer prizeAreaSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // ResizeSpriteToTargetSize(prizeScratchField.transform, prizeAreaSpriteRenderer, prizeAreaGridSize);
        // // prizeScratchField.transform.localScale = new Vector3(prizeAreaGridSize.x, prizeAreaGridSize.y, 1);
        // prizeScratchField.transform.SetParent(currentScratchCard.transform);
    }

    // void ResizeSpriteToTargetSize(Transform objectTransform, SpriteRenderer spriteRenderer, Vector2 targetSize)
    // {
    //     Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
    //
    //     float spriteWidth = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit;
    //     float spriteHeight = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit;
    //
    //     Vector2 scale = new Vector2(targetSize.x / spriteWidth, targetSize.y / spriteHeight);
    //     objectTransform.localScale = new Vector3(scale.x, scale.y, 1);
    //
    //     // Vector2 scale = new Vector2(targetSize.x / spriteSize.x, targetSize.y / spriteSize.y);
    //     //
    //     // objectTransform.localScale = new Vector3(scale.x, scale.y, 1f);
    // }
}
