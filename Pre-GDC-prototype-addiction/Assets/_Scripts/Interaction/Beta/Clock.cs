using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Manager;
using TMPro;
using UnityEngine;
using ResourceManager = Manager.ResourceManager;

public class Clock : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    
    private void OnEnable()
    {
        ResourceManager.OnTimeChanged += UpdateClock;
    }

    private void OnDisable()
    {
        ResourceManager.OnTimeChanged -= UpdateClock;
    }
    
    private void UpdateClock(DateTime newTime)
    {
        timeText.text = GameManager.Instance.resourceManager.CurrentTime.ToString("t");
    }
}
