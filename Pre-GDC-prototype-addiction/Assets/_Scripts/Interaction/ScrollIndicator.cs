using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interaction;
using UnityEngine;

public class ScrollIndicator : MonoBehaviour
{
    private ScrollView scrollView;
    private Transform currentDot;
    [SerializeField] private List<Transform> dotList;
    [SerializeField] private Transform mouse;

    void Start()
    {
        scrollView = GetComponentInParent<ScrollView>();
        currentDot = UpdateTargetDot();
        moveMouse(currentDot);
    }
    
    void Update()
    {
        if (scrollView.isScrollLocked) return;
        if (currentDot != UpdateTargetDot())
        {
            currentDot = UpdateTargetDot();
            moveMouse(currentDot);
        }
    }

    Transform UpdateTargetDot()
    {
        Transform targetDot;

        float scrollPercentage = Mathf.InverseLerp(scrollView.scrollLimits.y, scrollView.scrollLimits.x,
            scrollView.scrollViewHolder.transform.position.y);

        int targetDotIndex = Mathf.RoundToInt((dotList.Count - 1) * scrollPercentage);

        targetDot = dotList[targetDotIndex];

        return targetDot;
    }

    void moveMouse(Transform targetDot)
    {
        // if (Math.Abs(mouse.transform.position.y - targetDot.transform.position.y) < 0.001f) return;
        mouse.DOLocalMoveY(targetDot.transform.localPosition.y, 0.25f);
    }
}
