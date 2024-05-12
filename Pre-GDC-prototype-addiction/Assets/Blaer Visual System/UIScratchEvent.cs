using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Interaction;
using Cinemachine;
using DG.Tweening;
using Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Title,
        Quit,
        None,
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
                            Camera.main.DOOrthoSize(0.5f,2f).OnComplete(() => { SceneManager.LoadScene("Menu");});
                            break;
                        case TitleGoTo.Settings:
                            Camera.main.transform.DOMoveX(-19.2f, 1f).OnComplete(() =>
                            {
                                cardManager.Card.Clear();
                                cardManager.SpriteRendererCard.color = Color.white;
                                isFullyScratched = false;
                            });
                            break;
                        case TitleGoTo.Credits:
                            Camera.main.transform.DOMoveX(19.2f, 1f).OnComplete(() =>
                            {
                                cardManager.Card.Clear();
                                cardManager.SpriteRendererCard.color = Color.white;
                                isFullyScratched = false;
                            });
                            break;
                        case TitleGoTo.Title:
                            Camera.main.transform.DOMoveX(0f, 1f).OnComplete(() =>
                            {
                                cardManager.Card.Clear();
                                cardManager.SpriteRendererCard.color = Color.white;
                                isFullyScratched = false;
                            });
                            break;
                        case TitleGoTo.Quit:
                            break;
                        case TitleGoTo.None:
                            break;
                    }
                });
            }
        }
    }
}
