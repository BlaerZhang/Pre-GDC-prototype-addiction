using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchCardGenerator : MonoBehaviour
{
    [Header("Prize Generation")]
    public float winningProbability;

    [Header("Prize Distribution")]
    // public int prizeDistribution;

    [Header("Icon Placement")]
    public List<GameObject> iconPrefabs;

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
        DistributeIcons();
    }

    void Update()
    {
        
    }

    GameObject DistributeIcons()
    {
        PlaceIcons(targetAreaStartPosition, targetAreaGridSize.x, targetAreaGridSize.y);
        PlaceIcons(prizeAreaStartPosition, prizeAreaGridSize.x, prizeAreaGridSize.y);
        return iconPrefabs[0];
    }

    void PlaceIcons(Vector2 startPosition, int xGridAmount, int yGridAmount)
    {
        for (int i = 0; i < xGridAmount; i++)
        {
            for (int j = 0; j < yGridAmount; j++)
            {
                Vector2 cellPosition = new Vector2(i * cellSize + gapLength * i, j * cellSize + gapLength * j) + startPosition;
                Instantiate(iconPrefabs[0], cellPosition, Quaternion.identity);
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
