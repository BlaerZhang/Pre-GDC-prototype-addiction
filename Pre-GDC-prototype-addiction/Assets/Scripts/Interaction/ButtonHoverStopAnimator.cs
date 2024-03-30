using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverStopAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator buttonAnimator;

    private void Start()
    {
        buttonAnimator = GetComponentInParent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonAnimator.speed = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonAnimator.speed = 1;
    }
}
