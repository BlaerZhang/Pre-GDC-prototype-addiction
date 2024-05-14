using System.Collections;
using System.Collections.Generic;
using _Scripts.Manager;
using UnityEngine;

public class TitleGameManager : GameManager
{
    
    private void Awake()
    {
        Instance = this;
        audioManager = GetComponentInChildren<AudioManager>();
    }
}
