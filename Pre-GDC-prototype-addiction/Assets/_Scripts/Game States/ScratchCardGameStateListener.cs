using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.PlayerTools.Payphone;
using ScratchCardAsset;
using UnityEngine;

public class ScratchCardGameStateListener : MonoBehaviour
{
    private ScratchCardManager scratchCardManager;

    private void OnEnable()
    {
        scratchCardManager = GetComponent<ScratchCardManager>();
        PayphoneManager.onPhoneStateChanged += isInMessage => { if (scratchCardManager) scratchCardManager.InputEnabled = !isInMessage; };
    }
    
    private void OnDisable()
    {
        PayphoneManager.onPhoneStateChanged -= isInMessage => { if (scratchCardManager) scratchCardManager.InputEnabled = !isInMessage; };
    }
}
