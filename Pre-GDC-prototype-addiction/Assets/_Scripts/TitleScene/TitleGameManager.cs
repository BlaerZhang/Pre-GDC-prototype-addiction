using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Manager;
using UnityEngine;

public class TitleGameManager : GameManager
{
    public bool fullScreen = true;
    public Vector2 targetAspectRatio = new Vector2(16, 9);
    private void Awake()
    {
        Instance = this;
        audioManager = GetComponentInChildren<AudioManager>();
    }

    private void Start()
    {
        // Calculate the target height based on the screen width and 16:9 aspect ratio
        int targetHeight = Mathf.RoundToInt(Screen.width * targetAspectRatio.y / targetAspectRatio.x);
 
        // Set the game's resolution to match the target width and height
        Screen.SetResolution(Screen.width, targetHeight, fullScreen);
    }
}
