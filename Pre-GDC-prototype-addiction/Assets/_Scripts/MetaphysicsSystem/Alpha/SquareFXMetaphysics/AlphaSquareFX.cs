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
    [HideInInspector] public Vector2Int currentGrid;

    [HideInInspector] public bool isPositive = true;

    // private static bool _moveEndEventHasTriggered = false;
    // private static bool _splitTriggered = false;
    // private bool isMoving = false;
    // public static Action onFXMoveEnd;

    // [Header("Action Settings")]
    // public float moveDuration = 0.1f;

    public AlphaSquareFX Replicate(Vector2 initPosition, Vector2Int initGrid)
    {
        GameObject newFX = Instantiate(gameObject, initPosition, Quaternion.identity);
        AlphaSquareFX newAlphaSquareFX = newFX.GetComponent<AlphaSquareFX>();
        newAlphaSquareFX.currentGrid = initGrid;
        newAlphaSquareFX.transform.SetParent(GameObject.Find("currentScratchCard").transform);
        newAlphaSquareFX.isPositive = isPositive;
        return newAlphaSquareFX;
    }



    // TODO: test if the action could only be called once
    // TODO: test if the lock is unlocked after action
    /// <summary>
    /// move randomly to a not fully scratched grid
    /// </summary>
    // private void MoveRandomly()
    // {
    //     Vector2Int newGrid = Utils.SelectRandomGridFromMatrix(gridSize.x, gridSize.y);
    //     Vector2 newWorldPosition = gridPositionMatrix.GetElement(newGrid) + (Vector2)transform.parent.position;
    //     transform.DOMove(newWorldPosition, moveDuration)
    //         .OnStart(() =>
    //         {
    //             isMoving = true;
    //         })
    //         .OnComplete(() =>
    //         {
    //             isMoving = false;
    //             if (!_moveEndEventHasTriggered)
    //             {
    //                 _moveEndEventHasTriggered = true;
    //                 onFXMoveEnd?.Invoke();
    //                 StartCoroutine(ResetActionTrigger(.1f, _moveEndEventHasTriggered));
    //             }
    //         });
    // }

    // IEnumerator ResetActionTrigger(float waitTime, bool actionFlag)
    // {
    //     yield return new WaitForSeconds(waitTime);
    //     actionFlag = false;
    // }

}
