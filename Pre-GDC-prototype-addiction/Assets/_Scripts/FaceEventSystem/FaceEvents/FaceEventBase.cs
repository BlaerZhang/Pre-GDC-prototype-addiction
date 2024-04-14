using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.CustomEventSystem;
using Unity.VisualScripting;
using UnityEngine;

public class FaceEventBase : GameEventListenerBase
{
    public FaceEventType faceEventType;
    public int eventDuration = 3;
    public int strength = 0;
    public Action<FaceEventType, int> onEventTriggered;
    public override void OnEventRaised()
    {
        onEventTriggered?.Invoke(faceEventType, eventDuration);
        Debug.Log($"{faceEventType} triggered");
    }
}
