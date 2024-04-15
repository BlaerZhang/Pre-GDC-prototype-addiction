using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interaction;
using Manager;
using ScratchCardGeneration;
using ScratchCardGeneration.LayoutConstructor;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class AlphaSquareFX : SerializedBehaviour
{
    private FruitiesLayoutConstructor fruitiesLayoutConstructor;

    // if the grid is true, means it has been fully scratched
    private VariableMatrix<bool> scratchingStatusMatrix;

    // static
    private List<Vector2Int> prizeWinningGridList;
    private VariableMatrix<Vector2> gridPositionList;

    private Vector2Int currentGrid;

    [HideInInspector] public int spawnOrder = 0;

    [HideInInspector] public bool isPositive = true;

    [Header("Scratch no FX and FX not on winning grid")]
    public Dictionary<string, float> ScratchNoFXAndFXNotOnWinningGridProbabilityList;
    [Header("Scratch no FX and FX on winning grid")]
    public Dictionary<string, float> ScratchNoFXAndFXOnWinningGridProbabilityList;
    [Header("Scratch FX but FX not on winning grid (only for negative FX)")]
    public Dictionary<string, float> ScratchFXAndFXOnWinningGridProbabilityList;

    void Start()
    {
        fruitiesLayoutConstructor = (FruitiesLayoutConstructor)GameManager.Instance.scratchCardGenerator.cardLayoutConstructorDic[ScratchCardBrand.Fruities];
        // fruitiesLayoutConstructor.
    }

    private void OnEnable()
    {
        PrizeRevealing.onFullyScratched += TriggerFXAction;
    }

    private void OnDisable()
    {
        PrizeRevealing.onFullyScratched -= TriggerFXAction;
    }

    public void Spawn(Vector2 initPosition, int initSpawnOrder, bool initIsPositive)
    {
        spawnOrder = initSpawnOrder;
        isPositive = initIsPositive;
        Instantiate(gameObject, initPosition, Quaternion.identity);
    }

    private void TriggerFXAction()
    {
        if (isPositive)
        {

        }
        else
        {

        }
    }

    private void MoveToGrid(Vector2Int newGrid)
    {
        transform.position = gridPositionList.GetElement(newGrid);

        // check if multiple vfx are on the same grid

        // delete them until only one left
    }

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

        notFullyScratchedGridNearby.Add(rightGrid);
        notFullyScratchedGridNearby.Add(leftGrid);
        notFullyScratchedGridNearby.Add(upGrid);
        notFullyScratchedGridNearby.Add(downGrid);

        foreach (var grid in notFullyScratchedGridNearby.ToList())
        {
            if (scratchingStatusMatrix.GetElement(grid))
                notFullyScratchedGridNearby.Remove(grid);
        }

        // randomly choose one position to generate a new vfx
        if (notFullyScratchedGridNearby.Count == 0) return;
        int randIndex = Random.Range(0, notFullyScratchedGridNearby.Count);
        Vector2Int spawnGrid = notFullyScratchedGridNearby[randIndex];
        Vector2 spawnPosition = gridPositionList.GetElement(spawnGrid);

        Spawn(spawnPosition, spawnOrder + 1, isPositive);
    }
}
