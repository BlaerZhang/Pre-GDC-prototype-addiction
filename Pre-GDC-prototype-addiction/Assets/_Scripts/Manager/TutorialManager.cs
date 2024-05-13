using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Manager;
using _Scripts.PlayerTools.Payphone;
using Sirenix.OdinInspector;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Title("Opening")] 
    [SerializeField] private AudioClip payphoneRingSound;
    [SerializeField] private AudioClip payphonePickUpSound;
    
    private StatsTrackingManager statsTrackingManager;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    void Start()
    {
        statsTrackingManager = GetComponent<StatsTrackingManager>();
    }
    
    void Update()
    {
        
    }

    void Opening(int messageIndex)
    {
        GameObject ringAudio = GameManager.Instance.audioManager.PlayLoopSound(payphoneRingSound);
        //turn on light
        
    }
}
