using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using DG.Tweening;
using Manager;
using TMPro;
using UnityEngine;
using ResourceManager = Manager.ResourceManager;

public class Clock : MonoBehaviour
{
    public TextMeshProUGUI hourText;
    public TextMeshProUGUI colun;
    public TextMeshProUGUI minuteText;
    private DateTime displayTime;
    private int displayHour;
    private int displayMinute;

    private void OnEnable()
    {
        ResourceManager.OnTimeChanged += UpdateClock;
    }

    private void OnDisable()
    {
        ResourceManager.OnTimeChanged -= UpdateClock;
    }

    private void Start()
    {
        displayTime = GameManager.Instance.resourceManager.CurrentTime;
        displayHour = displayTime.Hour;
        displayMinute = displayTime.Minute;
    }

    private void UpdateClock(DateTime newTime)
    {
        displayTime = newTime;
        DOVirtual.Int(displayMinute, newTime.Minute, 1, value =>
        {
            minuteText.text = value.ToString("D2");
        }).OnComplete(() => { displayMinute = newTime.Minute; });

        DOVirtual.Int(displayHour, newTime.Hour, 1, value =>
        {
            hourText.text = value.ToString("D2");
        }).OnComplete(() => { displayHour = newTime.Hour; });
    }
}
