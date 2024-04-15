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
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class AlphaSquareFX : SerializedMonoBehaviour
{
    public enum FXActionName
    {
        Split,
        Disappear,
        Stay,
        Move
    }

    private FruitiesLayoutConstructor fruitiesLayoutConstructor;
    private Vector2Int gridSize;

    // prize grid
    // if the grid is true, means it has been fully scratched
    private VariableMatrix<bool> scratchingStatusMatrix;
    private List<Vector2Int> prizeWinningGridList;
    private VariableMatrix<Vector2> gridPositionMatrix;
    [HideInInspector] public Vector2Int currentGrid;

    [HideInInspector] public bool isPositive = true;

    private static bool _moveEndEventHasTriggered = false;
    private bool isMoving = false;
    public static Action onFXMoveEnd;

    [Header("Action Settings")]
    public float moveDuration = 0.1f;

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

    void Awake()
    {
        fruitiesLayoutConstructor = (FruitiesLayoutConstructor)GameManager.Instance.scratchCardGenerator.CardLayoutConstructorDic[ScratchCardBrand.Fruities];
        prizeWinningGridList = fruitiesLayoutConstructor.prizeWinningGridList;
        gridPositionMatrix = fruitiesLayoutConstructor.PrizeCellPositionMatrix;
        scratchingStatusMatrix = fruitiesLayoutConstructor.ScratchingStatusMatrix;
        gridSize = fruitiesLayoutConstructor.prizeAreaGridSize;

        transform.SetParent(GameObject.Find("currentScratchCard").transform);
        InitPosition();
    }

    private void OnEnable()
    {
        PrizeRevealing.onFullyScratched += TriggerFXAction;
    }

    private void OnDisable()
    {
        PrizeRevealing.onFullyScratched -= TriggerFXAction;
    }

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

    private void Spawn(Vector2 initPosition, bool initIsPositive)
    {
        GameObject newFX = Instantiate(gameObject, initPosition, Quaternion.identity);
        newFX.GetComponent<AlphaSquareFX>().isPositive = initIsPositive;
    }

    private void InitPosition()
    {
        currentGrid = Utils.SelectRandomGridFromMatrix(gridSize.x, gridSize.y);
        transform.localPosition = gridPositionMatrix.GetElement(currentGrid);

        print("current grid: " + currentGrid);
    }

    // triggered when fully scratched
    private void TriggerFXAction(Vector2Int currentFullyScratchedGrid)
    {
        if (isMoving) return;

        if (isPositive)
        {
            // FX ~ fully scratched grid
            if (currentGrid.Equals(currentFullyScratchedGrid))
            {
                print(prizeWinningGridList.Contains(currentGrid)
                    ? nameof(PositiveScratchFXAndFXOnWinningGridProbabilityList)
                    // Destroy(gameObject);
                    : nameof(PositiveScratchFXButFXNotOnWinningGridProbabilityList));

                // FX ~ prize grid
                ApplyProbabilityList(prizeWinningGridList.Contains(currentGrid)
                    ? PositiveScratchFXAndFXOnWinningGridProbabilityList
                    // Destroy(gameObject);
                    : PositiveScratchFXButFXNotOnWinningGridProbabilityList);
                // MoveRandomly();
            }
            else
            {
                print(prizeWinningGridList.Contains(currentGrid)
                    ? nameof(PositiveScratchNoFXButFXOnWinningGridProbabilityList)
                    // Destroy(gameObject);
                    : nameof(PositiveScratchNoFXAndFXNotOnWinningGridProbabilityList));
                // FX ~ prize grid
                ApplyProbabilityList(prizeWinningGridList.Contains(currentGrid)
                    ? PositiveScratchNoFXButFXOnWinningGridProbabilityList
                    : PositiveScratchNoFXAndFXNotOnWinningGridProbabilityList);
            }
        }
        else
        {
            // FX ~ fully scratched grid
            if (currentGrid.Equals(currentFullyScratchedGrid))
            {
                print(prizeWinningGridList.Contains(currentGrid)
                    ? nameof(NegativeScratchFXAndFXOnWinningGridProbabilityList)
                    // Destroy(gameObject);
                    : nameof(NegativeScratchFXButFXNotOnWinningGridProbabilityList));
                // FX ~ prize grid
                ApplyProbabilityList(prizeWinningGridList.Contains(currentGrid)
                    ? NegativeScratchFXAndFXOnWinningGridProbabilityList
                    // Destroy(gameObject);
                    : NegativeScratchFXButFXNotOnWinningGridProbabilityList);
            }
            else
            {
                print(prizeWinningGridList.Contains(currentGrid)
                    ? nameof(NegativeScratchNoFXButFXOnWinningGridProbabilityList)
                    // Destroy(gameObject);
                    : nameof(NegativeScratchNoFXAndFXNotOnWinningGridProbabilityList));
                // FX ~ prize grid
                ApplyProbabilityList(prizeWinningGridList.Contains(currentGrid)
                    ? NegativeScratchNoFXButFXOnWinningGridProbabilityList
                    : NegativeScratchNoFXAndFXNotOnWinningGridProbabilityList);
            }
        }
    }

    private void ApplyProbabilityList(Dictionary<FXActionName, float> dict)
    {
        FXActionName actionName = Utils.CalculateMultiProbability(dict);
        print($"actionName: {actionName}");

        switch (actionName)
        {
            case FXActionName.Disappear:
                Destroy(gameObject);
                break;
            case FXActionName.Split:
                SplitSelf();
                break;
            case FXActionName.Stay:
                break;
            case FXActionName.Move:
                MoveRandomly();
                break;
        }
    }

    // TODO: test if the action could only be called once
    // TODO: test if the lock is unlocked after action
    /// <summary>
    /// move randomly to a not fully scratched grid
    /// </summary>
    private void MoveRandomly()
    {
        Vector2Int newGrid = Utils.SelectRandomGridFromMatrix(gridSize.x, gridSize.y);
        Vector2 newWorldPosition = gridPositionMatrix.GetElement(newGrid) + (Vector2)transform.parent.position;
        print(newWorldPosition);
        print(gridPositionMatrix.GetElement(newGrid));
        transform.DOMove(newWorldPosition, moveDuration)
            .OnStart(() =>
            {
                isMoving = true;
            })
            .OnComplete(() =>
            {
                isMoving = false;
                if (!_moveEndEventHasTriggered)
                {
                    _moveEndEventHasTriggered = true;
                    onFXMoveEnd?.Invoke();
                    StartCoroutine(ResetActionTrigger(.1f));
                }
            });
    }

    IEnumerator ResetActionTrigger(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _moveEndEventHasTriggered = false;
    }

    /// <summary>
    /// spawn a new fx to a nearby not fully scratched grid
    /// </summary>
    private void SplitSelf()
    {
        // get nearby grids that are not fully scratched
        List<Vector2Int> notFullyScratchedGridNearby = new List<Vector2Int>();

        int gridX = currentGrid.x;
        int gridY = currentGrid.y;

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
                notFullyScratchedGridNearby.Remove(grid);
        }

        // randomly choose one position to generate a new vfx
        if (notFullyScratchedGridNearby.Count == 0) return;
        int randIndex = Random.Range(0, notFullyScratchedGridNearby.Count);
        Vector2Int spawnGrid = notFullyScratchedGridNearby[randIndex];
        Vector2 spawnPosition = gridPositionMatrix.GetElement(spawnGrid);

        Spawn(spawnPosition, isPositive);
    }
}
