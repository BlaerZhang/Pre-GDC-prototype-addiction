using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Interaction;
using UnityEngine;

public class UIScratchEvent : ScratchProgressEvent
{
    public float fullyScratchedThreshold = 0.7f;
    private bool isFullyScratched = false;
    public float autoRevealingDuration = 1f;
    public TitleGoTo titleGoTo;

    public enum TitleGoTo
    {
        Game,
        Settings,
        Credits,
        Quit
    }
    protected override void OnScratchProgress(float progress)
    {
        if (progress >= fullyScratchedThreshold)
        {
            if (!isFullyScratched)
            {
                isFullyScratched = true;

                cardManager.SpriteRendererCard?.DOFade(0, autoRevealingDuration).OnComplete(() =>
                {
                    cardManager.Card.Fill();
                    switch (titleGoTo)
                    {
                        case TitleGoTo.Game:
                            Camera.main.transform.DOMoveY(-1, 2f);
                            Camera.main.DOOrthoSize(0.5f,2f);
                            break;
                        case TitleGoTo.Settings:
                            break;
                        case TitleGoTo.Credits:
                            break;
                        case TitleGoTo.Quit:
                            break;
                    }
                });
            }
            base.OnScratchProgress(progress);
        }
    }
}
