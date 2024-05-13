using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Manager;
using _Scripts.PlayerTools.Payphone;
using Abu;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Title("Opening")] 
    [SerializeField] private AudioClip payphoneRingSound;
    [SerializeField] private AudioClip payphonePickUpSound;
    [SerializeField] private Volume payphoneVolume;
    [SerializeField] private GameObject raycastBlocker;
    [SerializeField] private RawImage mask;

    [Title("Opening Highlights")] 
    [SerializeField] private TutorialHighlight clockHighlight;
    [SerializeField] private TutorialHighlight moneyHighlight;
    [SerializeField] private List<TutorialHighlight> fruitiePosterHighlights;
    [SerializeField] private List<TutorialHighlight> toolsHighlights;
    
    private StatsTrackingManager statsTrackingManager;
    private bool isPickedUp = false;
    private bool isLightOn = false;
    private GameObject ringAudioTemp;

    private void OnEnable()
    {
        PayphoneManager.onSingleLineBegins += Opening;
    }

    private void OnDisable()
    {
        PayphoneManager.onSingleLineBegins -= Opening;
    }

    void Start()
    {
        statsTrackingManager = GetComponent<StatsTrackingManager>();
        isPickedUp = false;
        isLightOn = false;
        mask.enabled = true;

        Opening(-1);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPickedUp && isLightOn)
        {
            isPickedUp = true;
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
                mask.DOFade(230f / 256, 2f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    mask.color = Color.clear;
                    isLightOn = true;
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
                break;
        }
    }
}
