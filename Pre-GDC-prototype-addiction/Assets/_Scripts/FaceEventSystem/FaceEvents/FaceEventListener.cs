using System;
using _Scripts.CustomEventSystem;
using _Scripts.Interaction.PosterPicking;
using _Scripts.Manager;
using _Scripts.ScratchCardGeneration;
using UnityEngine;

namespace _Scripts.FaceEventSystem.FaceEvents
{
    public class FaceEventListener : GameEventListenerBase
    {
        public FaceEventType faceEventType;
        public int eventDuration = 3;
        public int strength = 0;
        public AudioClip triggerAudio;
        public static Action<FaceEventType, int, ScratchCardTier> onFaceEventTriggered;
        public override void OnEventRaised()
        {
            if (faceEventType == FaceEventType.NoEvent) return;
        
            onFaceEventTriggered?.Invoke(faceEventType, eventDuration, ScratchCardDealer.currentPickedCardTier);
        
            GameManager.Instance.audioManager.PlaySound(triggerAudio);
            Debug.Log($"{faceEventType} triggered");
        }
    }
}
