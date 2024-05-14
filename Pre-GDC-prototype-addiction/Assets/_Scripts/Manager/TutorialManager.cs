using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Interaction;
using _Scripts.Interaction.PosterPicking;
using _Scripts.Manager;
using _Scripts.PlayerTools.Payphone;
using Abu;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Title("Opening")] 
    [SerializeField] private AudioClip payphoneRingSound;
    [SerializeField] private AudioClip payphonePickUpSound;
    [SerializeField] private AudioClip lightOnSound;
    [SerializeField] private Volume payphoneVolume;
    [SerializeField] private GameObject raycastBlocker;
    [SerializeField] private RawImage mask;

    [Title("Opening Highlights")] 
    [SerializeField] private TutorialHighlight clockHighlight;
    [SerializeField] private TutorialHighlight moneyHighlight;
    [SerializeField] private List<TutorialHighlight> fruitiePosterHighlights;
    [SerializeField] private List<TutorialHighlight> toolsHighlights;
    private TutorialHighlight winningGridHighlight;

    [Title("First Card Highlights")] 
    [SerializeField] private TutorialHighlight prizeGridHighlight;
    [SerializeField] private GameObject targetGridHighlights;
    
    private StatsTrackingManager statsTrackingManager;
    private bool hasPayphonePickedUp = false;
    private bool isLightOn = false;
    private bool hasCardZoomedForFirstTime = false;
    private bool hasWonForFirstTime = false;
    private GameObject ringAudioTemp;

    private void OnEnable()
    {
        PayphoneManager.onSingleLineBegins += Opening;
        ScratchCardDealer.onToScratchStage += (brand, i, arg3, arg4) =>
        {
            if (hasCardZoomedForFirstTime) return;
            hasCardZoomedForFirstTime = true;
            PayphoneManager.onSingleLineBegins += FirstCard;
            raycastBlocker.SetActive(true);
            PayphoneManager.onPhoneStateChanged?.Invoke(true);
            DOVirtual.DelayedCall(1, () => PayphoneManager.onPhoneMessageSent?.Invoke("First Card")).Play();
        };
        PrizeRevealing.onWinningGridFullyScratched += grid =>
        {
            if (hasWonForFirstTime) return;
            hasWonForFirstTime = true;
            PayphoneManager.onSingleLineBegins += FirstPrize;
            raycastBlocker.SetActive(true);
            PayphoneManager.onPhoneStateChanged?.Invoke(true);
            winningGridHighlight = grid.GetComponentInChildren<SpriteRenderer>().AddComponent<TutorialHighlight>();
            winningGridHighlight.enabled = false;
            DOVirtual.DelayedCall(0.5f, () => PayphoneManager.onPhoneMessageSent?.Invoke("First Prize")).Play();
        };
    }

    private void OnDisable()
    {
        PayphoneManager.onSingleLineBegins -= Opening;
        ScratchCardDealer.onToScratchStage -= (brand, i, arg3, arg4) =>
        {
            if (hasCardZoomedForFirstTime) return;
            hasCardZoomedForFirstTime = true;
            PayphoneManager.onSingleLineBegins += FirstCard;
            raycastBlocker.SetActive(true);
            PayphoneManager.onPhoneStateChanged?.Invoke(true);
            DOVirtual.DelayedCall(1, () => PayphoneManager.onPhoneMessageSent?.Invoke("First Card")).Play();
        };
        PrizeRevealing.onWinningGridFullyScratched -= grid =>
        {
            if (hasWonForFirstTime) return;
            hasWonForFirstTime = true;
            PayphoneManager.onSingleLineBegins += FirstPrize;
            PayphoneManager.onPhoneMessageSent?.Invoke("First Prize");
            winningGridHighlight = grid.GetComponent<SpriteRenderer>().AddComponent<TutorialHighlight>();
            winningGridHighlight.enabled = false;
        };
    }

    void Start()
    {
        statsTrackingManager = GetComponent<StatsTrackingManager>();
        hasPayphonePickedUp = false;
        isLightOn = false;
        hasCardZoomedForFirstTime = false;
        hasWonForFirstTime = false;
        mask.enabled = true;

        Opening(-1);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasPayphonePickedUp && isLightOn)
        {
            hasPayphonePickedUp = true;
            GameManager.Instance.audioManager.StopLoopSound(ringAudioTemp);
            GameManager.Instance.audioManager.PlaySound(payphonePickUpSound);
            PayphoneManager.onPhoneMessageSent?.Invoke("Opening");
        }
    }

    void Opening(int messageIndex)
    {
        switch (messageIndex)
        {
            case -1:
                raycastBlocker.SetActive(true);
                PayphoneManager.onPhoneStateChanged?.Invoke(true);
                ringAudioTemp = GameManager.Instance.audioManager.PlayLoopSound(payphoneRingSound);
                mask.DOFade(250f / 256, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    DOVirtual.DelayedCall(1.5f, () =>
                    {
                        mask.color = Color.clear;
                        isLightOn = true;
                        GameManager.Instance.audioManager.PlaySound(lightOnSound);
                    }).Play();
                });
                break;
            case 4:
                payphoneVolume.enabled = false;
                clockHighlight.enabled = true;
                moneyHighlight.enabled = true;
                HighlightManager.SmoothEnableHighlight(0.5f);
                break;
            case 5:
                payphoneVolume.enabled = true;
                clockHighlight.enabled = false;
                moneyHighlight.enabled = false;
                HighlightManager.SmoothDisableHighlight(0f);
                break;
            case 6:
                payphoneVolume.enabled = false;
                foreach (var highlight in fruitiePosterHighlights) highlight.enabled = true;
                HighlightManager.SmoothEnableHighlight(0.5f);
                break;
            case 8:
                foreach (var highlight in fruitiePosterHighlights) highlight.enabled = false;
                foreach (var highlight in toolsHighlights) highlight.enabled = true;
                HighlightManager.SmoothDisableEnableCombo(0.5f);
                break;
            case 10:
                payphoneVolume.enabled = true;
                foreach (var highlight in toolsHighlights) highlight.enabled = false;
                HighlightManager.SmoothDisableHighlight(0f);
                PayphoneManager.onSingleLineBegins -= Opening;
                break;
        }
    }

    void FirstCard(int messageIndex)
    {
        switch (messageIndex)
        {
            case 2:
                payphoneVolume.enabled = false;
                prizeGridHighlight.enabled = true;
                targetGridHighlights.SetActive(true);
                HighlightManager.SmoothEnableHighlight(0.5f);
                break;
            case 3:
                prizeGridHighlight.enabled = false;
                HighlightManager.SmoothDisableEnableCombo(0.25f);
                break;
            case 4:
                prizeGridHighlight.enabled = true;
                targetGridHighlights.SetActive(false);
                HighlightManager.SmoothDisableEnableCombo(0.25f);
                break;
            case 6:
                payphoneVolume.enabled = true;
                prizeGridHighlight.enabled = false;
                HighlightManager.SmoothDisableHighlight(0.03f);
                PayphoneManager.onSingleLineBegins -= FirstCard;
                break;
        }
    }

    void FirstPrize(int messageIndex)
    {
        switch (messageIndex)
        {
            case 1:
                payphoneVolume.enabled = false;
                winningGridHighlight.enabled = true;
                HighlightManager.SmoothEnableHighlight(0.5f);
                break;
            case 4:
                payphoneVolume.enabled = true;
                Destroy(winningGridHighlight);
                HighlightManager.SmoothDisableHighlight(0.03f);
                PayphoneManager.onSingleLineBegins -= FirstPrize;
                break;
        }
    }

    void Coin(int messageIndex)
    {
        switch (messageIndex)
        {
            case 0:
                payphoneVolume.enabled = false;
                toolsHighlights[0].enabled = true;
                HighlightManager.SmoothEnableHighlight(0.5f);
                break;
            case 1:
                // -=
                break;
        }
    }
}