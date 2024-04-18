using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MaskFadeIn : MonoBehaviour
{
    private RawImage mask;
    void Start()
    {
        mask = GetComponent<RawImage>();
        mask.enabled = true;
        mask.DOFade(0, 2f).SetEase(Ease.Linear);
    }
}
