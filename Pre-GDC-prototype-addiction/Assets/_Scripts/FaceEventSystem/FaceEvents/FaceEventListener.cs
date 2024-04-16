using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.CustomEventSystem;
using Manager;
using Unity.VisualScripting;
using UnityEngine;

public class FaceEventListener : GameEventListenerBase
{
    public FaceEventType faceEventType;
    public int eventDuration = 3;
    public int strength = 0;
    public static Action<FaceEventType, int, ScratchCardTier> onFaceEventTriggered;
    public override void OnEventRaised()
    {
        if (faceEventType == FaceEventType.NoEvent) return;
        
        onFaceEventTriggered?.Invoke(faceEventType, eventDuration, GameManager.Instance.lastPickTier);
        Debug.Log($"{faceEventType} triggered");
    }
}
