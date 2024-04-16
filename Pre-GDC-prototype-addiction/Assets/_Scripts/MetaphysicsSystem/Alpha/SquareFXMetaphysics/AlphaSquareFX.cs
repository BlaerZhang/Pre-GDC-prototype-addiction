using UnityEngine;

public class AlphaSquareFX : MonoBehaviour
{
    [HideInInspector] public Vector2Int currentGrid;

    [HideInInspector] public bool isPositive = true;

    public AlphaSquareFX Replicate(Vector2 initPosition, Vector2Int initGrid)
    {
        GameObject newFX = Instantiate(gameObject);
        newFX.transform.position = initPosition;
        AlphaSquareFX newAlphaSquareFX = newFX.GetComponent<AlphaSquareFX>();
        newAlphaSquareFX.currentGrid = initGrid;
        newAlphaSquareFX.transform.SetParent(GameObject.Find("currentScratchCard").transform);
        newAlphaSquareFX.isPositive = isPositive;
        return newAlphaSquareFX;
    }
}
